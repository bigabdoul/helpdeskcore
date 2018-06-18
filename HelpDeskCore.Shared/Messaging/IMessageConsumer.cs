using System.Collections.Generic;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Specifies the data contract required for an object used to enqueue messages and send them out at a given moment.
    /// </summary>
    public interface IMessageConsumer : IEnumerable<INotificationMessage>
    {
        /// <summary>
        /// Enqueue a notification message.
        /// </summary>
        /// <param name="message">The notification message to enqueue.</param>
        void Enqueue(INotificationMessage message);

        /// <summary>
        /// Dequeue and return all notification messages currently stored in the message queue.
        /// </summary>
        /// <returns></returns>
        IEnumerable<INotificationMessage> DequeueAll();

        /// <summary>
        /// Notify the consumer that new messages have been enqueued and are ready to be sent.
        /// </summary>
        void Notify();
    }
}
