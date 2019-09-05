using System.Threading.Tasks;
using HelpDeskCore.Data;
using HelpDeskCore.Extensions;
using HelpDeskCore.Models;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HelpDeskCore.Data.Extensions;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Authorize(Policy = "ApiUser")]
  [Route("api/[controller]/[action]")]
  public class DashboardController : HelpDeskControllerBase
  {
    public DashboardController(UserManager<AppUser> userManager
      , ApplicationDbContext appDbContext
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , ISysEventLogger sysLogger)
      : base(appDbContext, identitySettings, httpContextAccessor, sysLogger)
    {
    }

    // GET api/dashboard/index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var emp = await FindEmployeeAsync();
      var role = emp.User.UserRole();

      return new OkObjectResult(new
      {
        Message = $"{WelcomeOnHelpDesk} {emp.User.FirstName}",
        emp.User.Id,
        emp.User.FirstName,
        emp.User.LastName,
        emp.User.PictureUrl,
        emp.User.FacebookId,
        emp.Location,
        emp.Locale,
        emp.Gender,
        Type = emp.User.UserType(),
        role,
        Admin = role == "admin",
        SuperAdmin = await IsBuiltInAdmin(),
      });
    }
  }
}
