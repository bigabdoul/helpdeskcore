namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Encapsulates notification event data.
    /// </summary>
    public class NotificationEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventArgs"/> class.
        /// </summary>
        public NotificationEventArgs(INotificationMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public virtual INotificationMessage Message { get; }
    }
}
