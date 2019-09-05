using System.Threading;
using System.Threading.Tasks;
using MailkitTools;
using MailkitTools.Services;
using MimeKit;

namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// Represents an object that sends e-mails.
  /// </summary>
  public class EmailSender : IEmailSender
  {
    bool _initialized;
    readonly IEmailClientService _emailClient;
    IEmailConfigurationProvider _settingsFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSender"/> class.
    /// </summary>
    /// <param name="emailClient">An object used to send emails.</param>
    /// <param name="settingsFactory">An object used to retrieve email configuration settings.</param>
    public EmailSender(IEmailClientService emailClient, IEmailConfigurationProvider settingsFactory)
    {
      _emailClient = emailClient;
      _settingsFactory = settingsFactory;
    }
    
    /// <summary>
    /// Gets the e-mail client used to send messages.
    /// </summary>
    protected IEmailClientService Client { get => _emailClient; }

    /// <summary>
    /// Performs internal one-time initializations.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task InitAsync()
    {
      if (_initialized) return;
      _emailClient.Configuration = await _settingsFactory.GetConfigurationAsync();
      _initialized = true;
    }

    /// <summary>
    /// Updates the configuration settings used to connect with the underlying <see cref="IEmailClientService"/>.
    /// </summary>
    /// <param name="config">The new configuration to set.</param>
    public virtual void ChangeConfiguration(IEmailClientConfiguration config)
    {
      _emailClient.Configuration = config;
    }

    /// <summary>
    /// Asynchronously sends an e-mail using the specified parameters.
    /// </summary>
    /// <param name="subject">The subject of the message.</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="from">A comma- (or semi-colon) separated list of addresses in the 'From' header.</param>
    /// <param name="to">A comma- (or semi-colon) separated list of addresses in the 'To' header.</param>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    public async Task SendAsync(string subject, string body, string from, string to, CancellationToken cancellationToken = default(CancellationToken)) => await SendAsync(EmailClientService.CreateMessage(subject, body, from, to), cancellationToken);

    /// <summary>
    /// Send the specified message asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    public virtual async Task SendAsync(MimeMessage message, CancellationToken cancellationToken = default(CancellationToken))
    {
      await InitAsync();
      await _emailClient.SendAsync(message, cancellationToken);
    }
  }
}
