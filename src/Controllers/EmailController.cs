using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using HelpDeskCore.ViewModels.Emails;
using MailKit;
using MailKit.Security;
using MailkitTools;
using MailkitTools.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Authorize(Policy = "ApiUser")]
  [Route("api/[controller]/[action]")]
  public class EmailController : Controller
  {
    readonly IEmailClientService _emailClient;
    readonly IEmailConfigurationProvider _emailClientSettingsFactory;

    public EmailController(IEmailClientService emailClient, IEmailConfigurationProvider emailClientSettingsFactory)
    {
      _emailClient = emailClient;
      _emailClientSettingsFactory = emailClientSettingsFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] EmailModel model)
    {
      return await SendMessageAsync(model);
    }

    [HttpPost]
    public async Task<IActionResult> Test([FromBody] TestEmailModel model)
    {
      return await SendMessageAsync(model.Message, model.Config);
    }

    private async Task<IActionResult> SendMessageAsync(EmailModel model, IEmailClientConfiguration config = null)
    {
      try
      {
        if (config == null)
          config = await _emailClientSettingsFactory.GetConfigurationAsync();

        _emailClient.Configuration = config;

        var message = EmailClientService.CreateMessage(
            model.Subject,
            model.Body,
            model.From,
            model.To
          );

        await _emailClient.SendAsync(message);
        return Ok();
      }
      catch(ServiceNotAuthenticatedException ex)
      {
        if (false == config?.RequiresAuth)
          return BadRequest(new ServiceNotAuthenticatedException(SmtpServerRequiresAuth));
        return BadRequest(ex);
      }
      catch(SslHandshakeException ex)
      {
        if (true == config?.UseSsl)
          return BadRequest(new SslHandshakeException(SmtpServerDoesNotSupportSsl));
        return BadRequest(ex);
      }
      catch(SocketException)
      {
        return BadRequest(new Exception(string.Format(SmtpHostUnreachable, config?.Host)));
      }
      catch (Exception ex)
      {
        return BadRequest(ex);
      }
    }
  }
}
