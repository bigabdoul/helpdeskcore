using System;

namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Specifies the contract required by an object used to log system-wide events.
    /// </summary>
    public interface ISysEventLogEntry
    {
        /// <summary>
        /// The identifier of the event log entry.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The message identifier of the event log entry.
        /// </summary>
        string MessageId { get; set; }

        /// <summary>
        /// The user identifier of the event log entry.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// The description of the event.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The type of event that occured.
        /// </summary>
        string EventType { get; set; }

        /// <summary>
        /// A custom data to store along with the log.
        /// </summary>
        string ObjectState { get; set; }

        /// <summary>
        /// The date and time the event occured.
        /// </summary>
        DateTime Date { get; set; }
    }
}
