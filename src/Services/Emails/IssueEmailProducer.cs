using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Data.Logging;
using HelpDeskCore.Services.Views;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.Shared.Messaging;
using HelpDeskCore.ViewModels.Emails;
using MailkitTools.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using static MailkitTools.Services.EmailClientService;

namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// Represents a scoped service that produces issue-related email notifications for subscribers.
  /// </summary>
  public sealed class IssueEmailProducer : MessageProducer, IIssueEmailProducer
  {
    const int PAGE_SIZE = 20;
    const string TEMPLATE_RECENT_COMMENTS = "~/Views/Emails/RecentComments.cshtml";

    readonly IRepository<Issue> _issueRepo;
    readonly IRepository<AppUser> _userRepo;
    readonly IRepository<IssueSubscriber> _subsRepo;
    readonly IRepository<Comment> _commentRepo;
    readonly IViewRenderService _emailTemplatesViewRender;

    bool _emailNotificationsEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssueEmailProducer"/> class using the specified parameters.
    /// </summary>
    /// <param name="logger">An object used to report incidences.</param>
    /// <param name="isseRepo">The issue repository.</param>
    /// <param name="userRepo">The application user repository.</param>
    /// <param name="subsRepo">The subscribers repository.</param>
    /// <param name="commentRepo">The comment repository.</param>
    /// <param name="templateViewRender">The e-mail template view render service.</param>
    public IssueEmailProducer(ILogger<IssueEmailProducer> logger, IRepository<Issue> isseRepo
      , IRepository<AppUser> userRepo
      , IRepository<IssueSubscriber> subsRepo
      , IRepository<Comment> commentRepo
      , IViewRenderService templateViewRender) : base(EmailDispatcher.Instance, logger)
    {
      _issueRepo = isseRepo;
      _userRepo = userRepo;
      _subsRepo = subsRepo;
      _commentRepo = commentRepo;
      _emailTemplatesViewRender = templateViewRender;
    }

    /// <summary>
    /// Asynchronously produces e-mails and notification messages for all intended recipients based on the specified event type.
    /// </summary>
    /// <param name="args">An object that holds issue-related event data.</param>
    /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
    /// <returns></returns>
    public override async Task ProcessAsync(SysEventArgs args, CancellationToken cancellationToken = default(CancellationToken))
    {
      // give a chance to the base message producer to process the event
      await base.ProcessAsync(args, cancellationToken);

      if (!(args is SysEventArgs<Issue> e)) throw new ArgumentException($"Must be of type {nameof(SysEventArgs<Issue>)}.", nameof(args));

      var issue = e.Data ?? throw new ArgumentNullException("Data");
      var user = (e.User as AppUser) ?? throw new ArgumentNullException("User");
      var settings = e.ObjectState as EmailSettingsViewModel ?? throw new ArgumentNullException("ObjectState", $"ObjectState must be of type {nameof(EmailSettingsViewModel)}");

      var notifs = settings.Notifications;
      _emailNotificationsEnabled = notifs.Enabled ?? false;

      try
      {
        if (issue.User == null && e.EventType != SysEventType.IssueDeleted)
        {
          issue = await _issueRepo.Query(q => q.QueryIssues(id: issue.Id, withIncludes: true)).SingleOrDefaultAsync();
        }

        var fullName = user.FullName();
        var userName = user.UserName;
        var notify = false;
        var outgoing = settings.Outgoing;
        var from = outgoing.FromDisplay;
        var useFromNameForAll = outgoing.UseFromNameForAll ?? false;
        var replyToAddrList = new InternetAddressList();

        if (!string.IsNullOrWhiteSpace(outgoing.ReplyTo))
          replyToAddrList.AddRange(outgoing.ReplyTo);

        EmailTemplate temp;
        var templates = settings.Templates;

        switch (e.EventType)
        {
          case SysEventType.IssueCreated:
            {
              // Sent to technicians when a new ticket arrives. All technicians that have permissions to the category get one of these.

              // Ticket confirmation notification  (the one users get after submitting a new ticket)?
              notify = notifs.TicketConfirmationNotification ?? false;

              if (notify && user.SendEmail && user.EmailConfirmed)
              {
                // "Ticket confirmation" email template: Sent to the ticket-submitter after the app received his ticket.
                temp = templates.TicketConfirmation;

                var m = CreateMessage(
                  temp.ReplaceSubject(issue.Subject).ToString(),
                  temp.ReplaceBody(issue.Body, issue.Subject)
                    .Replace("#Numero_ticket#", $"{issue.Id}")
                    .Replace("#Articles_Base_Connaissances#", string.Empty)
                    //.Replace("#Suggested_KB_articles#", string.Empty)
                    .ToString(),
                  from,
                  to: user.Email
                );

                m.ReplyTo.AddRange(replyToAddrList);
                Enqueue(WrapMessage(m, user.Id));
              }

              temp = templates.NewTicket;
              var subj = temp.ReplaceSubject(issue.Subject).ToString();
              var body = temp.ReplaceBody(issue.Body, issue.Subject).Replace("#Numero_ticket#", $"{issue.Id}").ToString();

              from = user.GetEmailAddress();

              // notify all admins?
              if (notifs.NotifyAllAdmins ?? false)
              {
                var qry = _userRepo.Query(q => q.NotDisabled().Admins().Not(user.Id).CanReceiveEmails());
                await ForUsersAsync(qry, subj, body, from, replyToAddrList, cancellationToken);
              }

              // notify techs in their categories?
              if (notifs.NotifyTechs ?? false)
              {
                var qry = _userRepo.Query(q => q.NotDisabled().Techs().Not(user.Id).CanReceiveEmails());
                await ForUsersAsync(qry, subj, body, from, replyToAddrList, cancellationToken);
              }
            }
            break;
          case SysEventType.IssueAssigned:
            {
              // Notify ALL technicians in a category when another technician TAKES a ticket
              temp = await TemplateForUpdate($"{fullName} a pris en charge la résolution du ticket #{issue.Id}.");

              if (user.IsTech && (notifs.NotifyAllTechsOnTechTakeOver ?? false))
              {
                await NotifyTechs(temp);
              }

              // notify ticket owner
              NotifyOwner(temp);
            }
            break;
          case SysEventType.IssueUpdated:
            {
              // Sent to both technicians and ticket-submitter (and all ticket-subscribers if any) when a new reply is added to the ticket
              temp = await TemplateForUpdate($"Le ticket #{issue.Id} a été mis à jour par {fullName}.");

              if (issue.UpdatedByUser)
              {
                from = user.GetEmailAddress();
                var techsNotified = false;

                // Notify ALL technicians in a category when a customer updates a ticket
                // (not just the ticket-technician and ticket-subscribers)?
                if (issue.UpdatedForTechView || (notifs.NotifyAllTechsOnCustomerUpdate ?? false))
                {
                  await NotifyTechs(temp);
                  techsNotified = true;
                }

                await NotifyAssignee(temp);

                // send to all subscribers but the submitter
                var qry = _subsRepo.Query(q => q.QueryIssueSubscribers(issue.Id).But(user.Id));

                if (techsNotified)
                  qry = qry.NotTechs(); // exclude the techs who've been notified previously

                await ForSubscribersAsync(qry, temp.Subject, temp.Body, from, replyToAddrList, cancellationToken);
              }
              else
              {
                // send to submitter
                NotifyOwner(temp);

                // send to subscribers except the owner and updater
                var qry = _subsRepo.Query(q => q.QueryIssueSubscribers(issue.Id).But(user.Id).But(issue.User.Id));

                await ForSubscribersAsync(qry, temp.Subject, temp.Body, from, replyToAddrList, cancellationToken);
              }
            }
            break;
          case SysEventType.IssueClosed:
            {
              // Sent to subscribers when a ticket is closed. Note that "Ticket closed notifications" setting has to be on.
              if (notifs.TicketClosedNotification ?? false)
              {
                temp = await TemplateForUpdate($"Le ticket #{issue.Id} a été fermé par {fullName}.");
                NotifyOwner(temp);
              }
            }

            break;
          case SysEventType.IssueReopened:
            // no template for this scenario?
            break;
          case SysEventType.IssueDeleted:
            // no template for this scenario?
            break;
          default:
            // definitely no template for this scenario!
            break;
        }

        Consumer.Notify();

        async Task<EmailTemplate> TemplateForUpdate(string whatHappened)
        {
          var comments = await _commentRepo.GetAsync(q => q.QueryCommentsForIssue(issue.Id).Skip(0).Take(3).ToArray());
          var recent = string.Empty;

          if (comments.Length > 0)
          {
            try
            {
              recent = await _emailTemplatesViewRender.RenderToStringAsync(TEMPLATE_RECENT_COMMENTS, comments);
            }
            catch (Exception ex)
            {
              Logger.LogWarning(ex, "An error occured while rendering the recent comments e-mail template.");

              var sb = new StringBuilder("<h3>Messages récents</h3>");

              foreach (var c in comments)
              {
                sb.AppendLine();
                sb.AppendLine(c.Body);
              }

              recent = sb.ReplaceLineBreaks().ToString().Trim();
            }
          }

          temp = templates.TicketUpdated;
          var subj = temp.ReplaceSubject(issue.Subject).ToString();
          var body = temp
            .ReplaceBody(issue.Body, issue.Subject)
            .Replace("#Quoi_De_Neuf#", whatHappened)
            .Replace("#Messages_recents#", recent)
            .Replace("#Categorie#", issue.Category?.Name)
            .Replace("#Statut#", issue.Status?.Name)
            .Replace("#Priorite#", UtilExtensions.PriorityName(issue.Priority))
            //.Replace("#What_Happened#", whatHappened)
            //.Replace("#Recent_messages#", recent)
            //.Replace("#Category#", catname)
            //.Replace("#Status#", statname)
            //.Replace("#Priority#", priority)
            .ToString();

          return new EmailTemplate { Body = body, Subject = subj };
        }

        async Task NotifyTechs(EmailTemplate et)
        {
          var qry = _userRepo.Query(q => q.NotDisabled().Techs().Not(user.Id).CanReceiveEmails());
          await ForUsersAsync(qry, et.Subject, et.Body, from, replyToAddrList, cancellationToken);
        }

        async Task NotifyAssignee(EmailTemplate et)
        {
          if (issue.IsAssigned())
          {
            var owner = await _userRepo.GetAsync(q => q.Find(issue.AssignedToUserId));
            if (owner != null)
            {
              EnqueueTemplate(et, owner);
            }
          }
        }

        void NotifyOwner(EmailTemplate et)
        {
          var owner = issue.User;
          if (owner.SendEmail && owner.Id != user.Id)
          {
            EnqueueTemplate(et, owner);
          }
        }

        void SetFromName(AppUser u)
        {
          if (!useFromNameForAll)
          {
            from = user.GetEmailAddress();
          }
        }

        void EnqueueTemplate(EmailTemplate et, AppUser owner)
        {
          SetFromName(owner);
          Enqueue(WrapMessage(CreateMessage(et.Subject, et.Body, from, owner.Email), owner.Id));
        }
      }
      catch (Exception ex)
      {
        Logger.LogError(ex, $"Error while producing issue e-mails in {nameof(ProcessAsync)}.");
      }
    }

    protected override string GetUserId(object user)
    {
      var cat = CurrentEvent.GetEventCategory();
      if (cat == SysEventCategory.Issue)
      {
        var iss = (Issue)CurrentEvent.Data;
        return iss?.UserId;
      }
      if (cat == SysEventCategory.User)
      {
        var u = (AppUser)CurrentEvent.Data;
        return u.Id;
      }
      return base.GetUserId(user);
    }

    protected override string GetUserName(object user)
    {
      var cat = CurrentEvent.GetEventCategory();
      if (cat == SysEventCategory.User)
      {
        var u = (AppUser)CurrentEvent.Data;
        return u.UserName;
      }
      return base.GetUserName(user);
    }

    #region helpers

    async Task ForUsersAsync(IQueryable<AppUser> qry, string subj, string body, string from, InternetAddressList replyTo, CancellationToken cancellationToken)
    {
      var page = 0;
      PagedList<AppUser> users;
      do
      {
        users = await qry.GetPageAsync(page++, PAGE_SIZE);
        CreateDispatch(users, subj, body, from, replyTo, cancellationToken);
      } while (users.MorePages);
    }

    async Task ForSubscribersAsync(IQueryable<IssueSubscriber> query
     , string subj
     , string body
     , string from
     , InternetAddressList replyTo
     , CancellationToken cancellationToken)
    {
      var page = 0;
      PagedList<IssueSubscriber> subscribers;
      do
      {
        subscribers = await query.GetPageAsync(page++, PAGE_SIZE);
        CreateDispatch(subscribers.Select(s => s.User), subj, body, from, replyTo, cancellationToken);
      } while (subscribers.MorePages);
    }

    void CreateDispatch(IEnumerable<AppUser> users
      , string subj
      , string body
      , string from
      , InternetAddressList replyTo
      , CancellationToken cancellationToken)
    {
      foreach (var u in users)
      {
        var m = CreateMessage(subj, body, from, u.Email);
        m.ReplyTo.AddRange(replyTo);
        Enqueue(WrapMessage(m, u.Id));
      }
    }

    MimeMessageContainer WrapMessage(MimeMessage message, string userId)
      => new MimeMessageContainer
      {
        Message = message,
        MessageId = message.MessageId,
        UserId = userId,
        EventType = CurrentEvent.EventType
      };

    void Enqueue(MimeMessageContainer container)
    {
      if (_emailNotificationsEnabled)
      {
        Consumer.Enqueue(container);
      }
      else
      {
        // TODO: store the e-mail?
      }
    }

    #endregion
  }
}
