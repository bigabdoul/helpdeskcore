using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Services.Notifications;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.Shared.Messaging;
using HelpDeskCore.ViewModels.Emails;
using MailkitTools;
using MailkitTools.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using static HelpDeskCore.Shared.Messaging.MessageProducer;
using static Newtonsoft.Json.JsonConvert;

namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// Represents a background task used to enqueue and broadcast messages.
  /// </summary>
  public sealed class EmailDispatcher : MessageConsumer
  {
    #region static & const

    static EmailDispatcher _instance;
    const int PAGE_SIZE = 20;
    const string EMAIL_FAILED = nameof(SysEventType.EmailSendFailed);

    #endregion

    #region fields

    bool _emailClientErrorSet;
    bool _updatingEmailSettings;
    EmailSettingsViewModel _emailSettings;

    // depency-injected
    readonly ISysEventLogger _sysLogger;
    readonly IEmailClientService _emailClient;
    readonly IRepository<SysEventLog> _sysRepo;
    readonly IRepository<AppSetting> _appSettingsRepo;
    readonly IHubContext<NotificationHub, INotificationHub> _hubContext;

    readonly object _emailSettingsSyncLock = new object();
    readonly HashSet<string> _failedMessages = new HashSet<string>();
    readonly HashSet<MimeMessageContainer> _workingSet = new HashSet<MimeMessageContainer>();
    //readonly ConcurrentQueue<MimeMessageContainer> _emailQ = new ConcurrentQueue<MimeMessageContainer>();

    #endregion

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailDispatcher"/> class using the specified parameter.
    /// </summary>
    /// <param name="emailClient">An object used to send e-mails.</param>
    /// <param name="emailSettingsProvider">An object used to obtain and refresh the e-mail configuration settings.</param>
    /// <param name="logger">An object used for logging.</param>
    /// <param name="options">The options to use for the current <see cref="IEmailDispatcher"/>.</param>
    /// <param name="sysEventRepo">The system event repository.</param>
    /// <param name="appSettingsRepo">The application settings repository.</param>
    /// <param name="sysLogger">The system event logger.</param>
    /// <param name="hubContext">The SignalR hub context.</param>
    public EmailDispatcher(IEmailClientService emailClient
      , ILogger<EmailDispatcher> logger
      , IOptions<MessageDispatchOptions> options
      , IRepository<SysEventLog> sysEventRepo
      , IRepository<AppSetting> appSettingsRepo
      , ISysEventLogger sysLogger
      , IHubContext<NotificationHub, INotificationHub> hubContext) : base(logger, options)
    {
      if (_instance != null) throw new InvalidOperationException($"No more than one instance is allowed for the singleton {nameof(EmailDispatcher)} service.");

      _instance = this;

      _emailClient = emailClient;
      _sysRepo = sysEventRepo;
      _appSettingsRepo = appSettingsRepo;
      _sysLogger = sysLogger;
      _hubContext = hubContext;

      SetEventHandlers()
        .Execute() // ensures that unsent e-mails stored in the database are requeued
        .StartTimer();
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the default instance of the <see cref="EmailDispatcher"/> class.
    /// </summary>
    public static EmailDispatcher Instance { get => _instance; }

    /// <summary>
    /// Gets the current e-mailconfiguration settings.
    /// </summary>
    internal EmailSettingsViewModel EmailSettings
    {
      get
      {
        if (_emailSettings == null)
        {
          lock (_emailSettingsSyncLock)
          {
            if (_emailSettings == null)
            {
              SyncEmailSettings().Wait();
            }
          }
        }
        return _emailSettings;
      }
    }

    #endregion
    
    #region overrides

    /// <summary>
    /// Executes a long running task until cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
      if (Dispatching || Disabled) return;
      Dispatching = true;
      CancellationTokenSource cts = null;

      try
      {
        await RetrieveFailedEmails();

        var unlogged = true;
        var hubClients = _hubContext.Clients.All;
        var messages = new HashSet<MimeMessage>();
        cts = CreateLinkedTokenSource(cancellationToken);
        
        foreach (var item in DequeueAll())
        {
          if (item is MimeMessageContainer container)
          {
            _workingSet.Add(container);
            messages.Add(container.Message);
          }
          else
          {
            try
            {
              // broadcast notification message
              await hubClients.BroadcastMessage(item);
            }
            catch (Exception ex)
            {
              if (unlogged)
              {
                Logger.LogWarning(ex, "Error broadcasting message to SignalR clients.");
                unlogged = false;
              }
            }
          }

          if (cts.IsCancellationRequested)
            break;
        }

        if (!cts.IsCancellationRequested && messages.Count > 0)
        {
          if (_emailClient.Configuration == null)
            await SyncEmailSettings();

          Logger.LogInformation("Dispatching e-mails...");

          await _emailClient.SendAsync(messages, cts.Token);

          Logger.LogInformation("Done dispatching e-mails.");
        }

        messages.Clear();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, "Something strange happened in the e-mail dispatcher.");
      }
      finally
      {
        _workingSet.Clear();
        cts?.Dispose();
        Dispatching = false;
      }
    }

    /// <summary>
    /// Unset event handlers and cancel any ongoing task.
    /// </summary>
    public override void Dispose()
    {
      try
      {
        UnsetEventHandlers().StopAsync(default(CancellationToken)).Wait();
      }
      catch
      {
      }
      base.Dispose();
    }

    #endregion

    #region helpers

    async Task SyncEmailSettings()
    {
      _updatingEmailSettings = true;
      try
      {
        _appSettingsRepo.DiscardChanges();
        var emailSettings = await _appSettingsRepo.GetAsync(q => q.GetEmailSettings());
        if (emailSettings != null)
        {
          _emailSettings = emailSettings.Value.Deserialize<EmailSettingsViewModel>();
          _emailClient.Configuration = _emailSettings.Smtp;
        }
      }
      catch
      {
      }
      finally
      {
        _updatingEmailSettings = false;
      }
    }

    async Task RetrieveFailedEmails()
    {
      try
      {
        var emails = await _sysRepo.GetAsync(q => q.Where(s => s.EventType == EMAIL_FAILED).ToArray());
        var firstError = true;

        foreach (var e in emails)
        {
          try
          {
            // deserialize using JSON.NET
            var m = DeserializeObject<OutgoingMessage>(e.ObjectState);
            Enqueue(new MimeMessageContainer
            {
              UserId = e.UserId,
              Message = EmailClientService.CreateMessage(m.Subject, m.Body, m.From, m.To, messageId: m.MessageId),
              MessageId = m.MessageId,
            });
          }
          catch (Exception ex)
          {
            if (firstError)
            {
              Logger.LogWarning(ex, "Error deserializing and creating MIME message from stored e-mail.");
              firstError = false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.LogCritical(ex, "Could not retrieve the failed stored e-mails.");
      }
    }

    async Task<bool> LogEmailFailure(string descr, MimeMessageContainer container, bool save = false)
    {

      try
      {
        var msg = container.Message;
        var msgId = msg.MessageId;

        if (! await LogContains(msgId))
        {
          var obj = new OutgoingMessage
          {
            MessageId = msgId,
            Subject = msg.Subject ?? string.Empty,
            Body = ((TextPart)msg.Body).Text ?? string.Empty,
            From = string.Join(",", msg.From.Select(addr => addr.ToString())),
            To = string.Join(",", msg.To.Select(addr => addr.ToString())),
          };
          _sysRepo.Add(new SysEventLog
          {
            MessageId = msgId,
            UserId = container.UserId,
            Description = descr,
            EventType = SysEventType.EmailSendFailed.ToString(),
            ObjectState = SerializeObject(obj),
          });

          if (save)
            await _sysRepo.SaveChangesAsync(CancellationTokenSource.Token);

          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Error in {nameof(LogEmailFailure)} while adding e-mail log.");
      }
      return false;
    }

    async Task<bool> LogContains(string messageId)
      => await _sysRepo.GetAsync(q => q.Where(e => e.MessageId == messageId).Any(), CancellationTokenSource.Token);

    async Task<SysEventLog> GetLog(string messageId)
      => await _sysRepo.GetAsync(q => q.Where(e => e.MessageId == messageId).FirstOrDefault(), CancellationTokenSource.Token);

    async Task RemoveLog(string messageId, bool save = false)
    {
      var msg = await GetLog(messageId);
      if (msg != null)
      {
        _sysRepo.Remove(msg);
        await _sysRepo.SaveChangesAsync(CancellationTokenSource.Token);
      }
    }

    bool FindContainer(string messageId, out MimeMessageContainer container)
    {
      container = _workingSet.FirstOrDefault(c => c.MessageId == messageId);
      return container != null;
    }

    #endregion

    #region event handlers

    Task TrackEmailConfigurationChange(object sender, SysEventArgs e)
    {
      if (!_updatingEmailSettings && e.EventType == SysEventType.EmailConfigUpdated)
      {
        Logger.LogInformation("E-mail configuration has been changed. Invaliding the application database context.");
        lock (_emailSettingsSyncLock)
        {
          return SyncEmailSettings();
        }
      }
      return Task.CompletedTask;
    }

    async Task EmailClient_Error(SendEventArgs e)
    {
      try
      {
        var msg = $"Could not send an e-mail. The message has been requeued.";
        var err = $"{msg }\r\n{e.Error.GetType().FullName}: {e.Error.Message}";

        if (!_emailClientErrorSet)
        {
          Logger.LogError(e.Error, msg);
          _emailClientErrorSet = true;
        }

        foreach (var m in e.Messages)
        {
          var mid = m.MessageId;
          if (FindContainer(mid, out var container))
          {
            Enqueue(container);

            if (!_failedMessages.Contains(mid))
            {
              await LogEmailFailure(msg, container);

              var notif = CreateNotification(err
                , container.UserId
                , SysEventType.EmailSendFailed
                , severity: MessageType.Error
                , messageId: mid);

              Enqueue(notif);

              _failedMessages.Add(mid);
            }
          }
        }

        await _sysRepo.SaveChangesAsync(CancellationTokenSource.Token);
      }
      catch
      {
      }
    }

    async Task EmailClient_Success(SendEventArgs e)
    {
      try
      {
        foreach (var m in e.Messages)
        {
          var id = m.MessageId;

          if (_failedMessages.Contains(id))
            _failedMessages.Remove(id);

          await RemoveLog(id);

          if (FindContainer(id, out var container))
            _workingSet.Remove(container);

          var notif = CreateNotification("E-mail envoyé avec succès."
                , container?.UserId
                , SysEventType.EmailSendSuccess
                , severity: MessageType.Success
                , messageId: id);

          Enqueue(notif);
        }

        await _sysRepo.SaveChangesAsync(CancellationTokenSource.Token);
      }
      catch (Exception ex)
      {
        Logger.LogWarning(ex, "Could not remove failed e-mail log entry.");
      }
    }

    // dispatch message to connected SignalR clients
    Task MessageDispatcher_Dispatch(object sender, NotificationEventArgs e)
      => _hubContext.Clients.All.BroadcastMessage(e.Message);

    EmailDispatcher SetEventHandlers()
    {
      _emailClient.Error += EmailClient_Error;
      _emailClient.Success += EmailClient_Success;
      _sysLogger.Logged += TrackEmailConfigurationChange;
      Dispatch += MessageDispatcher_Dispatch;
      ApplicationDbContext.Notify += TrackEmailConfigurationChange;
      return this;
    }

    EmailDispatcher UnsetEventHandlers()
    {
      _emailClient.Error -= EmailClient_Error;
      _emailClient.Success -= EmailClient_Success;
      _sysLogger.Logged -= TrackEmailConfigurationChange;
      Dispatch -= MessageDispatcher_Dispatch;
      ApplicationDbContext.Notify -= TrackEmailConfigurationChange;
      return this;
    }

    #endregion
  }
}
