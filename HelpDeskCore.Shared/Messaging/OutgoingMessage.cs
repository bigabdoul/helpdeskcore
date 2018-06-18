namespace HelpDeskCore.Shared.Messaging
{
  /// <summary>
  /// Represents an object used to serialize and deserialize messages.
  /// </summary>
  public class OutgoingMessage
  {
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// Gets or sets the message subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the 'From' address. Can be a comma- (or semi-colon) separated list of e-mail addresses.
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// Gets or sets the 'To' address. Can be a comma- (or semi-colon) separated list of e-mail addresses.
    /// </summary>
    public string To { get; set; }
  }
}
