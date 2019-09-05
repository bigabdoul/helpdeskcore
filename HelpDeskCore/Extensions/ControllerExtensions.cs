using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using HelpDeskCore.ViewModels.Emails;
using MailKit;
using MailKit.Security;
using MailkitTools.Services;
using Microsoft.AspNetCore.Mvc;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Extensions
{
  /// <summary>
  /// Provides extension methods for instances of the <see cref="Controller"/> class.
  /// </summary>
  public static class ControllerExtensions
  {
    /// <summary>
    /// Asynchronously sends an e-mail on behalf of the given controller using the specified parameters.
    /// </summary>
    /// <param name="controller">The controller that initiated the action.</param>
    /// <param name="model">An object used to create the message to send.</param>
    /// <param name="emailClient">An object used to send the e-mail.</param>
    /// <returns></returns>
    public static async Task<IActionResult> SendEmailAsync(this Controller controller, EmailModel model, IEmailClientService emailClient)
    {
      var config = emailClient.Configuration;
      try
      {
        var message = EmailClientService.CreateMessage(
            model.Subject,
            model.Body,
            model.From,
            model.To
          );

        await emailClient.SendAsync(message);
        return controller.Ok();
      }
      catch (ServiceNotAuthenticatedException ex)
      {
        if (false == config?.RequiresAuth)
          return controller.BadRequest(new ServiceNotAuthenticatedException(SmtpServerRequiresAuth));
        return controller.BadRequest(ex);
      }
      catch (SslHandshakeException ex)
      {
        if (true == config?.UseSsl)
          return controller.BadRequest(new SslHandshakeException(SmtpServerDoesNotSupportSsl));
        return controller.BadRequest(ex);
      }
      catch (SocketException)
      {
        return controller.BadRequest(new Exception(string.Format(SmtpHostUnreachable, config?.Host)));
      }
      catch (Exception ex)
      {
        return controller.BadRequest(ex);
      }
    }
  }
}
