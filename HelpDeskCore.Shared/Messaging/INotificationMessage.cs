using HelpDeskCore.Shared.Logging;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Specifies the data contract required for a notification message.
    /// </summary>
    public interface INotificationMessage
    {
        /// <summary>
        /// Get or set the message type.
        /// </summary>
        MessageType Type { get; set; }

        /// <summary>
        /// Get or set the event type.
        /// </summary>
        SysEventType EventType { get; set; }

        /// <summary>
        /// Get or set the message.
        /// </summary>
        object Message { get; set; }

        /// <summary>
        /// Get or set the message identifier.
        /// </summary>
        string MessageId { get; set; }

        /// <summary>
        /// Get or set the target user identifier.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Get or set the target user name.
        /// </summary>
        string UserName { get; set; }
    }
}
