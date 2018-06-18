using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Represents log entry for a system-wide event.
    /// </summary>
    public class SysEventLogEntry : ISysEventLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventLogEntry"/> class.
        /// </summary>
        public SysEventLogEntry()
        {
        }

        /// <summary>
        /// The identifier of the event log entry.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// The message identifier of the event log entry.
        /// </summary>
        [Required]
        [StringLength(128)]
        public virtual string MessageId { get; set; } = Guid.NewGuid().ToString().ToLowerInvariant();

        /// <summary>
        /// The user identifier of the event log entry.
        /// </summary>
        [Required]
        [StringLength(128)]
        public virtual string UserId { get; set; }

        /// <summary>
        /// The description of the event.
        /// </summary>
        [StringLength(500)]
        public virtual string Description { get; set; }

        /// <summary>
        /// The type of event that occured.
        /// </summary>
        [StringLength(50)]
        public virtual string EventType { get; set; }

        /// <summary>
        /// A custom data to store along with the log.
        /// </summary>
        public virtual string ObjectState { get; set; }

        /// <summary>
        /// The date and time the event occured.
        /// </summary>
        public virtual DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
