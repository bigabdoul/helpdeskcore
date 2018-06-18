using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static HelpDeskCore.Resources.Strings;
using static System.String;

namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Logs system-wide events into the database. This should be a transient service.
    /// </summary>
    public abstract class SysEventLoggerBase : ISysEventLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventLoggerBase"/> class using the specified parameters.
        /// </summary>
        protected SysEventLoggerBase()
        {
        }

        /// <summary>
        /// Event fired before an entry is logged.
        /// </summary>
        public event AsyncEventHandler<SysEventArgs> Logging;

        /// <summary>
        /// Event fired when an entry has been logged.
        /// </summary>
        public event AsyncEventHandler<SysEventArgs> Logged;

        /// <summary>
        /// Asynchronously adds an event entry to the database.
        /// </summary>
        /// <param name="type">The type of event to log.</param>
        /// <param name="user">The user who caused the event. Depending on the event type, this can be an instance of IdentityUser, the user id, or name.</param>
        /// <param name="data">Additional useful data (depending on the event type).</param>
        /// <param name="previousObjectState">The previous state of the data in case it was altered.</param>
        /// <returns></returns>
        public async Task LogAsync(SysEventType type, object user, object data = null, object previousObjectState = null)
          => await LogAsync(new SysEventArgs(type, user, data, previousObjectState));

        /// <summary>
        /// Asynchronously adds an event entry to the database.
        /// </summary>
        /// <param name="args">The arguments of the event to log.</param>
        /// <returns></returns>
        public virtual async Task LogAsync(SysEventArgs args)
        {
            try
            {
                await OnLoggingAsync(args);

                if (args.Cancel)
                {
                    return;
                }

                var type = args.EventType;
                var data = args.Data;
                var item = data as ILogItem;
                var dataId = item?.Id;
                var dataName = item?.Name;
                var state = args.ObjectState;
                var userName = GetUserName(type);
                var sysComment = false;
                string message = null;

                switch (type)
                {
                    case SysEventType.LoginFailure:
                        sysComment = true;
                        message = Format(SysEventLoginFailure, userName);
                        break;
                    case SysEventType.LoginSuccess:
                        message = Format(SysEventLoginSuccess, userName);
                        break;
                    case SysEventType.Logout:
                        message = Format(SysEventLogout, userName);
                        break;
                    case SysEventType.IssueCreated:
                        await AddSysCommentAsync(TicketCreated, dataId);
                        await AddEventEntryAsync(type, Format(SysEventIssueCreated, userName));
                        break;
                    case SysEventType.IssueAssigned:
                        sysComment = true;
                        message = Format(SysEventIssueAssigned, dataId, userName);
                        break;
                    case SysEventType.IssueUpdated:
                        message = Format(SysEventIssueUpdated, dataId, userName);
                        break;
                    case SysEventType.IssueClosed:
                        sysComment = true;
                        message = Format(SysEventIssueClosed, dataId, userName);
                        break;
                    case SysEventType.IssueReopened:
                        sysComment = true;
                        message = Format(SysEventIssueReopened, userName);
                        break;
                    case SysEventType.IssueDeleted:
                        message = Format(SysEventIssueDeleted, userName);
                        break;
                    case SysEventType.UserRegistered:
                        message = Format(SysEventUserRegistered, userName);
                        break;
                    case SysEventType.UserCreated:
                        message = Format(SysEventUserCreated, userName);
                        break;
                    case SysEventType.UserUpdated:
                        message = Format(SysEventUserUpdated, userName);
                        break;
                    case SysEventType.UserDeleted:
                        message = Format(SysEventUserDeleted, userName);
                        break;
                    case SysEventType.UserImported:
                        message = Format(SysEventUserImported, userName);
                        break;
                    case SysEventType.CategoryCreated:
                        message = Format(SysEventCategoryCreated, dataName);
                        break;
                    case SysEventType.CategoryUpdated:
                        message = Format(SysEventCategoryUpdated, dataName);
                        break;
                    case SysEventType.CategoryDeleted:
                        message = Format(SysEventCategoryDeleted, dataName);
                        break;
                    case SysEventType.EmailConfigUpdated:
                        message = SysEventEmailConfigUpdated;
                        break;
                    case SysEventType.EmailSendFailed:
                        message = SysEventEmailSendFailed;
                        break;
                    case SysEventType.EmailSendSuccess:
                        message = SysEventEmailSendSuccess;
                        break;
                    default:
                        break;
                }

                if (sysComment)
                {
                    await AddSysCommentAsync(message, dataId);
                }
                else if (message != null)
                {
                    await AddEventEntryAsync(type, message);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                OnLogged(args);
            }
        }

        /// <summary>
        /// Asynchronously adds a system comment.
        /// </summary>
        /// <param name="body">The comment text.</param>
        /// <param name="parentId">The identifier of an object to which the comment relates.</param>
        /// <returns></returns>
        protected abstract Task AddSysCommentAsync(string body, int? parentId = null);

        /// <summary>
        /// Asynchronously adds an event log entry using the underlying repository.
        /// </summary>
        /// <param name="type">The type of the event to log.</param>
        /// <param name="descr">The description of the event.</param>
        /// <param name="state">A custom object related to the event, which will be serialized.</param>
        /// <returns></returns>
        protected abstract Task AddEventEntryAsync(SysEventType type, string descr, object state = null);

        /// <summary>
        /// Returns the name of the user who caused the current event.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetUserName(SysEventType type);

        /// <summary>
        /// Asynchronously fires the <see cref="Logging"/> event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <returns></returns>
        protected virtual Task OnLoggingAsync(SysEventArgs args)
            => Logging != null ? Logging.Invoke(this, args) : Task.CompletedTask;

        /// <summary>
        /// Fires the <see cref="Logged"/> event.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        protected virtual void OnLogged(SysEventArgs args) => Logged?.Invoke(this, args);
    }
}
