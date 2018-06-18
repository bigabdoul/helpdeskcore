using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreTools.Tokens.Jwt;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.Shared.Messaging;
using HelpDeskCore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Route("api/[controller]")]
  public class AuthController : DataControllerBase
  {
    readonly IJwtFactory _jwtFactory;
    readonly JwtIssuerOptions _jwtOptions;
    readonly IMessageProducer _notifier;

    public AuthController(UserManager<AppUser> userManager
      , IJwtFactory jwtFactory
      , IOptions<JwtIssuerOptions> jwtOptions
      , ApplicationDbContext appDbContext
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , ISysEventLogger sysEventRegister
      , IMessageProducer notifier)
      : base(userManager, appDbContext, identitySettings, httpContextAccessor, sysEventRegister)
    {
      _jwtFactory = jwtFactory;
      _jwtOptions = jwtOptions.Value;
      _notifier = notifier;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
    {
      var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
      if (identity == null)
      {
        await EventLogger.LogAsync(SysEventType.LoginFailure, credentials.UserName);
        return BadRequest(ModelState.AddError("login_failure", BadLoginAttempt));
      }

      var userId = identity.Claims.Single(c => c.Type == "id").Value;
      var user = await FindUserAsync(userId);
      var jwt = await identity.GenerateJwtAsync(userId,
          credentials.UserName,
          user.UserRole(),
          _jwtFactory,
          _jwtOptions);

      await EventLogger.LogAsync(SysEventType.LoginSuccess, user, Request.Host.Host);
      return new OkObjectResult(jwt.Json());
    }

    [HttpPost("logout/{id}")]
    public async Task<IActionResult> PostLogout(string id)
    {
      try
      {
        var user = await FindUserAsync(id);
        if (user != null)
        {
          await EventLogger.LogAsync(SysEventType.Logout, user, Request.Host.Host);
        }
      }
      catch
      {
      }
      return Ok();
    }

    private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
    {
      if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        return null;

      // get the user to verifty
      var userToVerify = await UserManager.FindByNameAsync(userName);

      if (userToVerify == null) return null;

      // check the credentials
      if (await UserManager.CheckPasswordAsync(userToVerify, password))
      {
        if (userToVerify.Disabled)
        {
          if (!await IsBuiltInAdmin(userToVerify.Id))
          {
            return null;
          }
          else
          {
            // TODO: elaborate policy when built-in account is disabled but a successful login attempt was made.
          }
        }
        return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
      }

      // Credentials are invalid, or account doesn't exist
      return null;
    }

    protected override Task AfterEventLoggedAsync(object sender, SysEventArgs e) => _notifier.ProcessAsync(e);
  }
}
