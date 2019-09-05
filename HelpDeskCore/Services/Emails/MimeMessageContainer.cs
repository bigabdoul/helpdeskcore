using MimeKit;

namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// A simple wrapper for a <see cref="MimeMessage"/> object belonging to a given user.
  /// </summary>
  public class MimeMessageContainer : Shared.Messaging.NotificationMessage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MimeMessageContainer"/> class.
    /// </summary>
    public MimeMessageContainer()
    {
    }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public new MimeMessage Message { get => (MimeMessage)base.Message; set => base.Message = value; }
  }
}
