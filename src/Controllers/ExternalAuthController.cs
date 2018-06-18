using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoreTools.Tokens.Jwt;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Route("api/[controller]/[action]")]
  public class ExternalAuthController : Controller
  {
    private readonly ApplicationDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly FacebookAuthSettings _fbAuthSettings;
    private readonly IJwtFactory _jwtFactory;
    private readonly JwtIssuerOptions _jwtOptions;
    private static readonly HttpClient Client = new HttpClient();

    public ExternalAuthController(IOptions<FacebookAuthSettings> fbAuthSettingsAccessor, UserManager<AppUser> userManager, ApplicationDbContext appDbContext, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
    {
      _fbAuthSettings = fbAuthSettingsAccessor.Value;
      _userManager = userManager;
      _appDbContext = appDbContext;
      _jwtFactory = jwtFactory;
      _jwtOptions = jwtOptions.Value;
    }

    // POST api/externalauth/facebook
    [HttpPost]
    public async Task<IActionResult> Facebook([FromBody]FacebookAuthViewModel model)
    {
      // 1.generate an app access token
      var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
      var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
      // 2. validate the user access token
      var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
      var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

      if (!userAccessTokenValidation.Data.IsValid)
      {
        return BadRequest(Errors.AddError(ModelState, "login_failure", ExternalAuthInvalidFacebookToken));
      }

      // 3. we've got a valid token so we can request user data from fb
      var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={model.AccessToken}");
      var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

      // 4. ready to create the local user account (if necessary) and jwt
      var user = await _userManager.FindByEmailAsync(userInfo.Email);

      if (user == null)
      {
        var appUser = new AppUser
        {
          FirstName = userInfo.FirstName,
          LastName = userInfo.LastName,
          FacebookId = userInfo.Id,
          Email = userInfo.Email,
          UserName = userInfo.Email,
          PictureUrl = userInfo.Picture.Data.Url
        };

        var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

        if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrors(ModelState, result));

        await _appDbContext.Employees.AddAsync(new Employee { UserId = appUser.Id, Location = "", Locale = userInfo.Locale, Gender = userInfo.Gender });
        await _appDbContext.SaveChangesAsync();
      }

      // generate the jwt for the local user...
      var localUser = await _userManager.FindByNameAsync(userInfo.Email);

      if (localUser == null)
      {
        return BadRequest(Errors.AddError(ModelState, "login_failure", ExternalAuthLocalAccountCreationFailed));
      }

      var identity = _jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id);

      var jwt = await identity.GenerateJwtAsync(localUser.Id,
        localUser.UserName,
        localUser.UserRole(),
        _jwtFactory,
        _jwtOptions);

      return new OkObjectResult(jwt.Json());
    }
  }
}
