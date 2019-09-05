using System.Threading.Tasks;
using AutoMapper;
using CoreTools.Tokens.Jwt;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Extensions;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.Services.Views;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.ViewModels;
using HelpDeskCore.ViewModels.Emails;
using MailkitTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : DataControllerBase
    {
        const string EMAIL_TEMPLATE = "/Views/Emails/PasswordReset{0}.cshtml";
        readonly char DirSeparator = System.IO.Path.DirectorySeparatorChar;
        readonly IMapper _mapper;
        readonly IViewRenderService _templateViewRender;
        readonly IHostingEnvironment _hostingEnvironment;
        readonly IEmailClientService _emailClient;
        readonly IEmailConfigurationProvider _emailClientSettingsFactory;

        public AccountsController(IEmailClientService emailClient
          , IEmailConfigurationProvider emailClientSettingsFactory
          , UserManager<AppUser> userManager
          , ApplicationDbContext dbContext
          , IMapper mapper
          , IOptions<IdentityInitializerSettings> identitySettings
          , IHttpContextAccessor httpContextAccessor
          , ISysEventLogger sysLog
          , IViewRenderService templateViewRender
          , IHostingEnvironment hostingEnvironment)
          : base(userManager, dbContext, identitySettings, httpContextAccessor, sysLog)
        {
            _mapper = mapper;
            _templateViewRender = templateViewRender;
            _hostingEnvironment = hostingEnvironment;
            _emailClient = emailClient;
            _emailClientSettingsFactory = emailClientSettingsFactory;

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

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel fpm)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(fpm.UserName);
                if (user != null)
                {
                    if (!user.Disabled)
                    {
                        var locale = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

                        // the localized e-mail template name
                        var viewName = string.Format(EMAIL_TEMPLATE, $".{locale}");

                        // check if the physical file path of the template exists
                        var path = _hostingEnvironment.ContentRootPath.TrimEnd(DirSeparator) + viewName.Replace('/', DirSeparator);

                        if (!System.IO.File.Exists(path))
                            // get the default email template
                            viewName = string.Format(EMAIL_TEMPLATE, ".fr");

                        var config = Startup.InternalConfiguration;
                        var model = new ResetPasswordViewModel
                        {
                            UserName = fpm.UserName,
                            AppName = config["ApplicationName"],
                            Token = await UserManager.GeneratePasswordResetTokenAsync(user),
                            BaseUrl = config.GetSection(nameof(JwtIssuerOptions))[nameof(JwtIssuerOptions.Audience)],
                        };

                        // render the email template
                        var body = await _templateViewRender.RenderToStringAsync(viewName, model);

                        // create the message.
                        var email = new EmailModel
                        {
                            Body = body,
                            Subject = ResetYourPassword,
                            From = Services.Emails.EmailDispatcher.Instance.EmailSettings.Outgoing.FromDisplay,
                            To = user.Email,
                        };

                        // get e-mail client config and send it
                        _emailClient.Configuration = await _emailClientSettingsFactory.GetConfigurationAsync();

                        return await this.SendEmailAsync(email, _emailClient);
                    }
                    else
                    {
                        // account disabled
                        return BadRequest(ModelState.AddError(string.Empty, UserAccountDoesNotExistOrDisabled));
                    }
                }
                else
                {
                    // for security reasons never reveal the account does not exist
                    return BadRequest(ModelState.AddError(string.Empty, UserAccountDoesNotExistOrDisabled));
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await UserManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var result = await UserManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
