using System.Threading;
using System.Threading.Tasks;

namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// Specifies the data contract required for a service that sends e-mails.
  /// </summary>
  public interface IEmailSender
  {
    /// <summary>
    /// Asynchronously send an e-mail using the specified parameters.
    /// </summary>
    /// <param name="subject">The subject of the message.</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="from">A comma- (or semi-colon) separated list of addresses in the 'From' header.</param>
    /// <param name="to">A comma- (or semi-colon) separated list of addresses in the 'To' header.</param>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    Task SendAsync(string subject, string body, string from, string to, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Send the specified message asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    Task SendAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken = default(CancellationToken));
  }
}
