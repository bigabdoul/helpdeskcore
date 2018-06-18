namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Specifies the data contract required for a notification hub.
    /// </summary>
    public interface INotificationHub
    {
        /// <summary>
        /// Broadcast a notification message to connected client applications.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <returns></returns>
        System.Threading.Tasks.Task BroadcastMessage(INotificationMessage message);
    }
}
