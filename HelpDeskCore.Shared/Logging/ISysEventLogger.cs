using System.Threading.Tasks;

namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Specifies the contract required by a service that logs and reports system-wide events.
    /// </summary>
    public interface ISysEventLogger
    {
        /// <summary>
        /// Asynchronously add an event entry to the data store.
        /// </summary>
        /// <param name="type">The type of event to log.</param>
        /// <param name="user">The user who caused the event. Depending on the event type, this can be an instance of an object, or the user identifier.</param>
        /// <param name="data">Additional useful data (depending on the event type).</param>
        /// <param name="previousObjectState">The previous state of the data in case it was altered.</param>
        /// <returns></returns>
        Task LogAsync(SysEventType type, object user, object data = null, object previousObjectState = null);

        /// <summary>
        /// Asynchronously add an event entry to the data store.
        /// </summary>
        /// <param name="args">The event data to log.</param>
        /// <returns></returns>
        Task LogAsync(SysEventArgs args);

        /// <summary>
        /// Event fired before an entry is logged.
        /// </summary>
        event AsyncEventHandler<SysEventArgs> Logging;

        /// <summary>
        /// Event fired when an entry has been logged.
        /// </summary>
        event AsyncEventHandler<SysEventArgs> Logged;
    }
}
