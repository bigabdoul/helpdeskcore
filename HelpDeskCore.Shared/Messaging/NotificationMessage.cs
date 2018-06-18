using HelpDeskCore.Shared.Logging;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// A simple wrapper for an object belonging to a given user.
    /// </summary>
    public class NotificationMessage : INotificationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
        /// </summary>
        public NotificationMessage()
        {
        }

        /// <summary>
        /// Gets or sets the type of event that created the current message.
        /// </summary>
        public SysEventType EventType { get; set; } = SysEventType.Unspecified;

        /// <summary>
        /// Gets or sets the type of the current message.
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Info;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the contained message.
        /// </summary>
        public virtual string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user whom the message belongs to.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user whom the message belongs to.
        /// </summary>
        public string UserName { get; set; }
    }
}
