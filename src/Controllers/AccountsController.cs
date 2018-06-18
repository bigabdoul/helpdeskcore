using System.Threading.Tasks;
using AutoMapper;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HelpDeskCore.Controllers
{
  [Route("api/[controller]")]
  public class AccountsController : DataControllerBase
  {
    private readonly IMapper _mapper;

    public AccountsController(UserManager<AppUser> userManager
      , ApplicationDbContext dbContext
      , IMapper mapper
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , ISysEventLogger sysLog)
      : base(userManager, dbContext, identitySettings, httpContextAccessor, sysLog)
    {
      _mapper = mapper;
    }
    
    // POST api/accounts
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
    {
      var appUser = _mapper.Map<AppUser>(model);

      var result = await UserManager.CreateAsync(appUser, model.Password);

      if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrors(ModelState, result));

      await Db.Employees.AddAsync(new Employee { UserId = appUser.Id, Location = model.Location });
      await Db.SaveChangesAsync();

      if (string.Equals(model.Mode, "admin", System.StringComparison.OrdinalIgnoreCase))
      {
        await EventLogger.LogAsync(SysEventType.UserCreated, await FindUserAsync(), appUser);
      }
      else
      {
        await EventLogger.LogAsync(SysEventType.UserRegistered, appUser);
      }

      return Ok();
    }
  }
}
