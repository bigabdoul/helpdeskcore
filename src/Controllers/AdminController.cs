using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.Services.Imports;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.Shared.Messaging;
using HelpDeskCore.ViewModels;
using HelpDeskCore.ViewModels.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static HelpDeskCore.Data.Extensions.EntityExtensions;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Produces("application/json")]
  [Authorize(Policy = "ApiUser")]
  [Route("api/[controller]")]
  public class AdminController : DataControllerBase
  {
    readonly IMapper _mapper;
    readonly IMessageProducer _notifier;

    public AdminController(UserManager<AppUser> userManager
      , ApplicationDbContext appDbContext
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , IMapper mapper
      , ISysEventLogger sysEventRegister
      , IMessageProducer notifier
      )
      : base(userManager, appDbContext, identitySettings, httpContextAccessor, sysEventRegister)
    {
      _mapper = mapper;
      _notifier = notifier;
    }

    #region user management

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
      var emp = await FindEmployeeAsync(model.UserId);
      if (emp?.User == null) return NotFound();

      var target = emp.User;
      var is_admin = await IsAdmin();
      var is_builtin = await IsBuiltInAdmin();
      var is_self = emp.UserId == GetUserId();
      var hasPwd = await UserManager.HasPasswordAsync(target);
      var oldPasswordHash = string.Empty;

      if (hasPwd)
        oldPasswordHash = target.PasswordHash;

      if (!is_self)
      {
        // somebody's trying to update another user

        if (!is_admin)
        {
          // a non-administrator cannot update another user
          return BadRequest();
        }

        if (!is_builtin && target.IsAdministrator)
        {
          // only the built-in administrator can modify another admin
          return BadRequest();
        }

        return await change_admin_pwd();
      }
      else if (is_admin || is_builtin)
      {
        return await change_admin_pwd();
      }
      else if (string.IsNullOrWhiteSpace(model.OldPassword))
      {
        return BadRequest(ModelState.AddError(string.Empty, ChangePasswordOldRequired));
      }

      // when simple users change their password
      IdentityResult result;

      if (hasPwd)
        result = await UserManager.ChangePasswordAsync(target, model.OldPassword, model.NewPassword);
      else
        result = await UserManager.AddPasswordAsync(target, model.NewPassword);

      if (result.Succeeded)
      {
        return await change_pwd_success();
      }

      return BadRequest(ModelState.AddError(string.Empty, ChangePasswordBadAttempt));

      // when an admin changes a user's password, no need to specify the old one
      async Task<IActionResult> change_admin_pwd()
      {
        IdentityResult res;
        if (hasPwd)
        {
          target.PasswordHash = UserManager.PasswordHasher.HashPassword(target, model.NewPassword);
          res= await UserManager.UpdateAsync(target);
        }
        else
        {
          res = await UserManager.AddPasswordAsync(target, model.NewPassword);
        }

        if (res.Succeeded)
          return await change_pwd_success();

        return BadRequest(ModelState.AddError(string.Empty, ChangePasswordFailed));
      }

      async Task<IActionResult> change_pwd_success()
      {
        var user = target;

        if (!is_self)
          user = await FindUserAsync();

        await EventLogger.LogAsync(SysEventType.UserPasswordChanged, user, target, new { oldPasswordHash });
        return Ok();
      }
    }

    // GET api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> Users([FromQuery] PaginationModel model)
    {
      IQueryable<AppUser> query = Db.Users;
      if (model.Column.HasValue)
      {
        query = query.Filter(model.Column.Value);
      }
      if (!string.IsNullOrWhiteSpace(model.Query))
      {
        query = query.Search(model.Query);
      }
      return await GetUserSnapshotAsync(query, model);
    }
    
    [HttpGet("userdetail/{id}")]
    public async Task<IActionResult> UserDetail(string id)
    {
      var emp = await FindEmployeeAsync(id, true);
      if (emp == null) return NotFound();
      // return detailed data
      return new OkObjectResult(emp.AsDetail());
    }

    [HttpPut("userdetail")]
    public async Task<IActionResult> UserDetail([FromBody] UserDetailViewModel model)
    {
      var emp = await FindEmployeeAsync(model.Id, true);
      if (emp == null) return NotFound();

      // now do some heavy-weight lifting
      var target = emp.User;
      var is_tech = target.IsTech;
      var is_self = emp.UserId == GetUserId();
      var is_admin = await IsAdmin();
      var is_builtin = await IsBuiltInAdmin();

      // check if logged-in user can modify this model
      if (!is_self)
      {
        if (!is_admin)
        {
          // a non-administrator cannot update another user
          return BadRequest();
        }

        if (target.IsAdministrator && ! is_builtin)
        {
          // only the built-in administrator can modify another admin
          return BadRequest();
        }
      }

      var oldEmp = emp.Clone();

      if (model.CompanyId == null || model.CompanyId < 0)
      {
        if (!string.IsNullOrWhiteSpace(model.CompanyName))
        {
          var comp = await Db.GetOrCreateCompanyAsync(model.CompanyName);
          emp.CompanyId = comp.Id;
        }
      }
      else
      {
        emp.CompanyId = model.CompanyId;
      }

      if (model.DepartmentId == null || model.DepartmentId < 0)
      {
        if (!string.IsNullOrWhiteSpace(model.DepartmentName))
        {
          try
          {
            var depmt = await Db.Departments.Where(d => d.Name == model.DepartmentName).FirstOrDefaultAsync();
            if (depmt == null)
            {
              depmt = new Department { Name = model.DepartmentName.Trim() };
              await Db.Departments.AddAsync(depmt);
              await Db.SaveChangesAsync();
            }
            emp.DepartmentId = depmt.Id;
          }
          catch (Exception ex)
          {
            System.Diagnostics.Trace.WriteLine(ex);
            throw;
          }
        }
      }
      else
      {
        emp.DepartmentId = model.DepartmentId;
      }

      emp.Gender = model.Gender;
      emp.Locale = model.Locale;
      emp.Location = model.Location;
      emp.PhoneNumberExtension = model.PhoneExtension;
      emp.Signature = model.Signature;

      target.Email = model.Email;
      target.FacebookId = model.FacebookId;
      target.FirstName = model.FirstName;
      target.LastName = model.LastName;
      target.Greeting = model.Greeting;
      target.Notes = model.Notes;
      target.PhoneNumber = model.Phone;
      target.PictureUrl = model.PictureUrl;
      target.SendEmail = model.SendEmail;
      target.TwoFactorEnabled = model.TwoFactor;

      if (is_tech || is_admin)
        target.SendNewTicketTechEmail = model.SendNewTicketTechEmail;
      else
        target.SendNewTicketTechEmail = false;

      // one cannot disable its own account (maybe the built-in only to avoid exploitation when not used?)
      if (!is_self && is_admin)
        target.Disabled = model.Disabled;

      // user roles update requirements:
      // 1. users must be administrators
      // 2. admins cannot update their own roles, even not the built-in (who would then perform system-wide troubleshooting?)
      if (is_admin && !is_self && !string.IsNullOrWhiteSpace(model.Role))
      {
        var update = true; // almost there

        if (!is_builtin && await IsAdmin(target.Id))
        {
          // 3. when target is admin then only built-in should be able to update it
          update = false;
        }

        if (update)
        {
          // good
          switch (model.Role.ToUpperInvariant())
          {
            case "ADMIN":
              target.IsAdministrator = true;
              target.IsTech = false;
              target.IsManager = false;
              break;
            case "TECH":
              target.IsAdministrator = false;
              target.IsTech = true;
              target.IsManager = false;
              break;
            default:
              target.IsAdministrator = false;
              target.IsTech = false;
              target.IsManager = model.IsManager;
              break;
          }
        }
      }

      await Db.SaveChangesAsync();
      await EventLogger.LogAsync(SysEventType.UserUpdated, await FindUserAsync(), target, oldEmp);
      return Ok();
    }

    [HttpPost("import-users"), DisableRequestSizeLimit]
    public async Task<IActionResult> ImportUsers()
    {
      try
      {
        var file = Request.Form.Files[0];
        if (file == null || file.Length <= 0L) return BadRequest(ModelState);

        var user = await FindUserAsync();
        if (!user.IsAdministrator) return BadRequest();

        var content = await new FormFileReader(file).ReadAsStringAsync();

        await new UserDataImporter(UserManager, Db, _mapper).ImportAsync(content
          , onCreating: au =>
          {
            au.SendEmail = true;
          }
          , onCreated: async au =>
          {
            await EventLogger.LogAsync(SysEventType.UserImported, user, au);
          }
          , onFailed: (ui, ir) =>
          {
          });
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }

      return Ok();
    }

    #endregion

    #region category management

    // GET api/admin/categories
    [HttpGet("categories")]
    public async Task<IActionResult> Categories([FromQuery] PaginationModel model)
    {
      var techOrAdmin = await Db.IsAdminOrTech(GetUserId());
      var p = await Db.GetCategoriesAsync(model.Page ?? 0
        , model.Size ?? 10
        , techOrAdmin
        , model.SortBy ?? 0
        , model.Query);

      return new OkObjectResult(new
      {
        p.PageCount,
        p.TotalCount,
        items = p.Items.Select(c => new
        {
          c.Id,
          c.Name,
        })
      });
    }

    [HttpGet("categorydetail/{id}")]
    public async Task<IActionResult> CategoryDetail(int id)
    {
      var cat = await Db.Categories.FindAsync(id);
      if (cat == null) return NotFound();
      // return detailed data
      return new OkObjectResult(cat.AsDetail());
    }

    [HttpPut("categorydetail")]
    public async Task<IActionResult> CategoryDetail([FromBody] CategoryDetailViewModel model)
    {
      try
      {
        var oldCat = await Db.Categories.FindAsync(model.Id);

        if (oldCat == null)
          return NotFound();

        // update the category accordingly
        var entity = _mapper.Map<Category>(model);

        if (!(model.DifferentFrom ?? false))
        {
          entity.FromAddress = null;
          entity.FromName = null;
          entity.FromAddressInReplyTo = false;
        }

        // detach the old category to avoid trouble
        Db.Entry(oldCat).State = EntityState.Detached;

        // set the new state for the entity
        Db.Entry(entity).State = EntityState.Modified;

        await Db.SaveChangesAsync();
        await EventLogger.LogAsync(SysEventType.CategoryUpdated, await FindUserAsync(), entity, oldCat);

        return Ok();
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }
    }
    #endregion

    #region email settings management

    const string EMAIL_SETTINGS_NAME = "email-settings";

    [HttpGet(EMAIL_SETTINGS_NAME)]
    public async Task<IActionResult> EmailConfig()
    {
      var setting = await Db.AppSettings.GetEmailSettingsAsync();

      EmailSettingsViewModel config = null;

      if (setting?.Value == null)
        config = new EmailSettingsViewModel();
      else
        config = setting.Value.Deserialize<EmailSettingsViewModel>();

      return new OkObjectResult(config);
    }

    [HttpPut(EMAIL_SETTINGS_NAME)]
    public async Task<IActionResult> EmailConfigUpdate([FromBody] EmailSettingsViewModel model)
    {
      if (model == null)
        return BadRequest(ModelState);

      AppSetting oldSetting = null;
      var setting = await Db.AppSettings.GetEmailSettingsAsync();

      if (setting == null)
      {
        setting = new AppSetting { Name = EMAIL_SETTINGS_NAME };
        await Db.AppSettings.AddAsync(setting);
      }
      else
      {
        oldSetting = setting.Clone();
      }

      setting.Value = Newtonsoft.Json.JsonConvert.SerializeObject(model);
      await Db.SaveChangesAsync();
      await EventLogger.LogAsync(SysEventType.EmailConfigUpdated, await FindUserAsync(), setting, oldSetting);

      return Ok();
    }

    #endregion

    // GET api/admin/sections
    [HttpGet("sections")]
    public async Task<IActionResult> Sections([FromQuery] PaginationModel model)
    {
      var page = model.Page ?? 0;
      var size = model.Size ?? 20;
      var sortBy = model.SortBy ?? 0;
      var desc = sortBy < 0;
      var p = await Db.GetSectionsAsync(page, size);
      return new OkObjectResult(new { p.TotalCount, p.PageCount, items = p.Items.Select(i => new { i.Id, i.Name }) });
    }

    // GET api/admin/departments
    [HttpGet("departments")]
    public async Task<IActionResult> Departments()
    {
      var items = await Db.Departments.OrderBy(d => d.Name).ToArrayAsync();
      return new OkObjectResult(items.Select(i => new { i.Id, i.Name }));
    }

    #region helpers

    protected override Task AfterEventLoggedAsync(object sender, SysEventArgs e) => _notifier.ProcessAsync(e);

    async Task<IActionResult> GetUserSnapshotAsync(IQueryable<AppUser> query, PaginationModel model)
    {
      var by = model.SortBy ?? 0;
      var page = model.Page ?? 0;
      var size = model.Size ?? 10;
      var p = await query.Sort(by).GetPageAsync(page, size);
      return new OkObjectResult(new { p.TotalCount, p.PageCount, items = p.Items.Select(u => u.AsSnapshot()) });
    }

    #endregion
  }
}
