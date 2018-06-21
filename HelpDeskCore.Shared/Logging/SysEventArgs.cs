using System;

namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Encapsulates data for system-wide events.
    /// </summary>
    public class SysEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventArgs"/> class using the specified parameter.
        /// </summary>
        /// <param name="type">The type of event.</param>
        public SysEventArgs(SysEventType type)
        {
            EventType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventArgs"/> class using the specified parameters.
        /// </summary>
        /// <param name="type">The type of event.</param>
        /// <param name="user">The user who caused the event.</param>
        /// <param name="data">The event data.</param>
        /// <param name="objectState">The previous state the event data.</param>
        public SysEventArgs(SysEventType type, object user, object data = null, object objectState = null)
        {
            EventType = type;
            User = user;
            Data = data;
            ObjectState = objectState;
        }

        /// <summary>
        /// Gets or sets the type of the current event.
        /// </summary>
        public virtual SysEventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the user who caused the event.
        /// </summary>
        public virtual object User { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        public virtual object Data { get; set; }

        /// <summary>
        /// Gets or sets the previous state of an object.
        /// </summary>
        public virtual object ObjectState { get; set; }

        /// <summary>
        /// Gets or sets the error that occured during an operation.
        /// </summary>
        public virtual Exception Error { get; set; }

        /// <summary>
        /// Returns the category of the current event.
        /// </summary>
        /// <returns></returns>
        public virtual SysEventCategory GetEventCategory() => GetEventCategory(EventType);

        /// <summary>
        /// Returns the category of the specified event type.
        /// </summary>
        /// <param name="type">The event type for which to return the category.</param>
        /// <returns></returns>
        public static SysEventCategory GetEventCategory(SysEventType type)
        {
            switch (type)
            {
                case SysEventType.LoginFailure:
                case SysEventType.LoginSuccess:
                case SysEventType.Logout: return SysEventCategory.Login;

                case SysEventType.IssueCreated:
                case SysEventType.IssueAssigned:
                case SysEventType.IssueUpdated:
                case SysEventType.IssueClosed:
                case SysEventType.IssueReopened:
                case SysEventType.IssueDeleted: return SysEventCategory.Issue;

                case SysEventType.UserRegistered:
                case SysEventType.UserCreated:
                case SysEventType.UserUpdated:
                case SysEventType.UserDeleted:
                case SysEventType.UserPasswordChanged: return SysEventCategory.User;

                case SysEventType.CategoryCreated:
                case SysEventType.CategoryUpdated:
                case SysEventType.CategoryDeleted: return SysEventCategory.Category;

                case SysEventType.EmailConfigUpdated:
                case SysEventType.EmailSendSuccess:
                case SysEventType.EmailSendFailed: return SysEventCategory.Email;

                default: return SysEventCategory.Unknown;
            }
        }
    }
}
