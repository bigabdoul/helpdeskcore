using System;
using System.Threading;
using System.Threading.Tasks;
using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using static HelpDeskCore.Resources.Strings;
using static System.String;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Represents an object that generates system event-related messages.
    /// </summary>
    public class MessageProducer : IMessageProducer
    {
        #region fields

        /// <summary>
        /// References the consumer used to enqueue produced messages.
        /// </summary>
        protected readonly IMessageConsumer Consumer;

        /// <summary>
        /// References the object used to log messages.
        /// </summary>
        protected readonly ILogger<MessageProducer> Logger;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProducer"/> class.
        /// </summary>
        /// <param name="consumer">The consumer used to enqueue produced messages.</param>
        /// <param name="logger">An object used to report incidences.</param>
        public MessageProducer(IMessageConsumer consumer, ILogger<MessageProducer> logger)
        {
            Logger = logger;
            Consumer = consumer;
        }

        #endregion

        /// <summary>
        /// Gets or sets the current event.
        /// </summary>
        protected SysEventArgs CurrentEvent { get; set; }

        /// <summary>
        /// Asynchronously produces notification messages for all intended recipients based on the specified event type.
        /// </summary>
        /// <param name="args">An object that holds the event data.</param>
        /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
        /// <returns></returns>
        public virtual Task ProcessAsync(SysEventArgs args, CancellationToken cancellationToken = default(CancellationToken))
        {
            CurrentEvent = args;
            if (args.EventType == SysEventType.Unspecified) return Task.CompletedTask;

            try
            {
                var userName = GetUserName(args.User);
                var targetId = GetUserId(args.User);
                var cat = args.GetEventCategory();

                if (cat == SysEventCategory.User && args.Data is IdentityUser iduser)
                {
                    userName = iduser.UserName;
                    targetId = iduser.Id;
                }

                int? dataId = null;
                var dataName = Empty;

                if (args.Data is ILogItem data)
                {
                    dataId = data.Id;
                    dataName = data.Name;
                }

                string message = null;
                MessageType messageType = MessageType.Info;

                switch (CurrentEvent.EventType)
                {
                    case SysEventType.LoginFailure:
                        messageType = MessageType.Warn;
                        message = Format(SysEventLoginFailure, userName);
                        break;
                    case SysEventType.LoginSuccess:
                        message = Format(SysEventLoginSuccess, userName);
                        break;
                    case SysEventType.Logout:
                        message = Format(SysEventLogout, userName);
                        break;
                    case SysEventType.IssueCreated:
                        message = Format(SysEventIssueCreated, userName);
                        break;
                    case SysEventType.IssueAssigned:
                        message = Format(SysEventIssueAssigned, dataId, userName);
                        break;
                    case SysEventType.IssueUpdated:
                        message = Format(SysEventIssueUpdated, dataId, userName);
                        break;
                    case SysEventType.IssueClosed:
                        message = Format(SysEventIssueClosed, dataId, userName);
                        break;
                    case SysEventType.IssueReopened:
                        message = Format(SysEventIssueReopened, userName);
                        break;
                    case SysEventType.IssueDeleted:
                        messageType = MessageType.Warn;
                        message = Format(SysEventIssueDeleted, userName);
                        break;
                    case SysEventType.UserRegistered:
                        message = Format(SysEventUserRegistered, userName);
                        break;
                    case SysEventType.UserCreated:
                        messageType = MessageType.Success;
                        message = Format(SysEventUserCreated, userName);
                        break;
                    case SysEventType.UserUpdated:
                        messageType = MessageType.Success;
                        message = Format(SysEventUserUpdated, userName);
                        break;
                    case SysEventType.UserDeleted:
                        messageType = MessageType.Warn;
                        message = Format(SysEventUserDeleted, userName);
                        break;
                    case SysEventType.UserImported:
                        message = Format(SysEventUserImported, userName);
                        break;
                    case SysEventType.CategoryCreated:
                        messageType = MessageType.Success;
                        message = Format(SysEventCategoryCreated, dataName);
                        break;
                    case SysEventType.CategoryUpdated:
                        messageType = MessageType.Success;
                        message = Format(SysEventCategoryUpdated, dataName);
                        break;
                    case SysEventType.CategoryDeleted:
                        messageType = MessageType.Warn;
                        message = Format(SysEventCategoryDeleted, dataName);
                        break;
                    case SysEventType.EmailConfigUpdated:
                        messageType = MessageType.Success;
                        message = SysEventEmailConfigUpdated;
                        break;
                    case SysEventType.EmailSendFailed:
                        messageType = MessageType.Warn;
                        message = SysEventEmailSendFailed;
                        break;
                    case SysEventType.EmailSendSuccess:
                        messageType = MessageType.Success;
                        message = SysEventEmailSendSuccess;
                        break;
                    default:
                        break;
                }
                if (message != null)
                {
                    Consumer.Enqueue(CreateNotification(message, targetId, messageType, userName));
                    Consumer.Notify();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, Empty);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Return the target user name.
        /// </summary>
        /// <param name="user">The user name, or an entity that represents a user.</param>
        /// <returns></returns>
        protected virtual string GetUserName(object user)
        {
            if (user is IdentityUser au)
                return au.UserName;
            else if (user is string name)
                return name;
            else if (user != null)
                return user.ToString();

            return Empty;
        }

        /// <summary>
        /// Return the identifier of the target user.
        /// </summary>
        /// <param name="user">The user identifier, or an entity that represents a user.</param>
        /// <returns></returns>
        protected virtual string GetUserId(object user)
        {
            if (user is IdentityUser au)
                return au.Id;
            else if (user is string name)
                return name;
            else if (user != null)
                return user.ToString();

            return Empty;
        }

        /// <summary>
        /// Creates a notification message using the current event data, and the specified parameters.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="userId">The identifier of user targeted by the message.</param>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="userName">The name of the target user.</param>
        /// <param name="messageId">The message identifier. If null, a new GUID is generated.</param>
        /// <returns></returns>
        protected virtual INotificationMessage CreateNotification(object message
            , string userId = null
            , MessageType severity = MessageType.Info
            , string userName = null
            , string messageId = null) => CreateNotification(message, userId, CurrentEvent.EventType, userName, severity, messageId);

        /// <summary>
        /// Creates a notification message using the specified parameters.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="userId">The identifier of user targeted by the message.</param>
        /// <param name="type">The event type.</param>
        /// <param name="userName">The name of the target user.</param>
        /// <param name="severity">The severity of the message.</param>
        /// <param name="messageId">The message identifier. If null, a new GUID is generated.</param>
        /// <returns></returns>
        public static INotificationMessage CreateNotification(object message
            , string userId
            , SysEventType type
            , string userName = null, MessageType severity = MessageType.Info
            , string messageId = null) => new NotificationMessage
            {
                EventType = type,
                Message = /*message is string ? $"[{DateTime.UtcNow.ToString("dd/MM/yyyy @ HH:mm:ss")}]\n{message}" :*/ message,
                MessageId = messageId ?? Guid.NewGuid().ToString(),
                Type = severity,
                UserId = userId,
            };
    }
}
