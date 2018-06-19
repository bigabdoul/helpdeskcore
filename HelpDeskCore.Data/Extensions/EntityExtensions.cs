using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared.Logging;
using Microsoft.EntityFrameworkCore;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Data.Extensions
{
    using static UtilExtensions;

    public static class EntityExtensions
    {
        static readonly string NEWLN = Environment.NewLine;

        #region File management

        public static byte[] SerializeAsArray(this Issue issue) => Encoding.UTF8.GetBytes(issue.Serialize());

        public static string Serialize(this Issue issue)
        {
            var sb = new StringBuilder(TicketUpdated + NEWLN + NEWLN);
            sb.AppendLine(PreviousSubject + NEWLN);
            sb.AppendLine(issue.Subject);
            sb.AppendLine();
            sb.AppendLine(PreviousContent + NEWLN);
            sb.AppendLine(issue.Body);
            return sb.ToString();
        }

        public static async Task<FileAttachment> AddFileAttachementAsync(this ApplicationDbContext context, Issue issue, int commentId, string userId, string fileName = "TicketEditLog.txt")
          => await context.AddFileAttachementAsync(issue.SerializeAsArray(), issue.Id, commentId, userId, fileName);

        public static async Task<FileAttachment> AddFileAttachementAsync(this ApplicationDbContext context, byte[] data, int issueId, int commentId, string userId, string fileName = "TicketEditLog.txt")
        {
            var fa = new FileAttachment
            {
                CommentId = commentId,
                FileData = data,
                FileHash = data.ComputeHash(),
                FileName = fileName,
                FileSize = data.Length,
                IssueId = issueId,
                UserId = userId,
            };
            await context.FileAttachments.AddAsync(fa);
            await context.SaveChangesAsync();
            return fa;
        }
        #endregion

        #region Issue and Comment extensions

        public static IQueryable<Issue> Search(this IQueryable<Issue> q, string terms)
        {
            var arr = terms.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // not very handsome but gets some results
            q = from e in q
                where e.Body.Contains(terms) | arr.Contains(e.Body) | e.Subject.Contains(terms) | arr.Contains(e.Subject)
                select e;
            return q;
        }

        public static bool IsAssigned(this Issue issue) => !string.IsNullOrWhiteSpace(issue.AssignedToUserId);

        /// <summary>
        /// Returns a query of active user subscriptions for the identified issue.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        /// <param name="issueId">The identifier of the issue for which to retrieve subscriptions.</param>
        /// <returns></returns>
        /// <remarks>Only subscriptions of active users willing to receive e-mail notifications are returned.</remarks>
        public static IQueryable<IssueSubscriber> QueryIssueSubscribers(this ApplicationDbContext context, int issueId)
            => context.Subscribers.QueryIssueSubscribers(issueId);

        public static IQueryable<IssueSubscriber> QueryIssueSubscribers(this IQueryable<IssueSubscriber> query, int issueId)
          => query
            .Include(s => s.User)
            .Where(i => i.IssueId == issueId)
            .Where(i => !i.User.Disabled && i.User.SendEmail);

        public static IQueryable<IssueSubscriber> But(this IQueryable<IssueSubscriber> query, string userId)
          => query.Where(i => i.UserId != userId);

        public static IQueryable<IssueSubscriber> Techs(this IQueryable<IssueSubscriber> query)
          => query.Where(i => i.User.IsTech);

        public static IQueryable<IssueSubscriber> NotTechs(this IQueryable<IssueSubscriber> query)
          => query.Where(i => !i.User.IsTech);

        public static IQueryable<IssueSubscriber> Admins(this IQueryable<IssueSubscriber> query)
          => query.Where(i => i.User.IsAdministrator);

        public static IQueryable<IssueSubscriber> NotAdmins(this IQueryable<IssueSubscriber> query)
          => query.Where(i => !i.User.IsAdministrator);

        public static async Task<Issue> GetIssueAsync(this ApplicationDbContext context, int id, string userId, bool withIncludes = true)
          // an admin or tech can access all tickets
          => await (await context.QueryIssuesForAdminOrTechAsync(userId, id, withIncludes)).SingleOrDefaultAsync();

        public static async Task<bool> UpdateIssueAsync(this ApplicationDbContext context
            , int issueId
            , string userId
            , bool closeTicket = false
            , bool save = false
            , TicketStatus? newStatus = null
            , bool? updatedForTechView = null
            , bool logUpdate = true) => await context.UpdateIssueAsync(await context.GetIssueAsync(issueId, userId, false)
                , userId
                , closeTicket
                , save
                , newStatus
                , updatedForTechView
                , logUpdate);

        public static async Task<bool> UpdateIssueAsync(this ApplicationDbContext context
            , Issue iss
            , string userId
            , bool closeTicket = false
            , bool save = false
            , TicketStatus? newStatus = null
            , bool? updatedForTechView = null
            , bool logUpdate = true)
        {
            if (iss == null) return false;

            var currentStatus = (TicketStatus)iss.StatusId;

            if (currentStatus == TicketStatus.Deleted)
                return false;

            var now = DateTime.UtcNow;
            var eventTriggered = false;

            if (newStatus != null)
            {
                if (newStatus.Value != TicketStatus.Unchanged)
                {
                    var inewStatus = (int)newStatus.Value;
                    if (iss.StatusId != inewStatus)
                    {
                        iss.StatusId = inewStatus;
                        if (currentStatus == TicketStatus.Resolved && newStatus.Value == TicketStatus.InProgress)
                        {
                            context.InvokeSysEvent(SysEventType.IssueReopened, userId, iss);
                            eventTriggered = true;
                        }
                        else if (currentStatus != TicketStatus.Resolved && newStatus.Value == TicketStatus.Resolved)
                        {
                            context.InvokeSysEvent(SysEventType.IssueClosed, userId, iss);
                            eventTriggered = true;
                        }
                    }
                }
            }
            else if (currentStatus == TicketStatus.New)
            {
                if (iss.StartDate == null) iss.StartDate = now;
                iss.StatusId = (int)TicketStatus.InProgress;
            }
            else if (currentStatus == TicketStatus.InProgress)
            {
                if (iss.StartDate == null) iss.StartDate = now;
            }

            iss.LastUpdated = now;
            iss.UpdatedByUser = iss.UserId == userId;
            iss.UpdatedByPerformer = iss.AssignedToUserId == userId;

            if (updatedForTechView.HasValue)
                iss.UpdatedForTechView = updatedForTechView.Value;

            var resolved = (int)TicketStatus.Resolved;

            if (closeTicket && iss.StatusId != resolved)
            {
                iss.StatusId = resolved;
                if (newStatus == null)
                {
                    context.InvokeSysEvent(SysEventType.IssueClosed, userId, iss);
                    eventTriggered = true;
                }
            }

            if (save)
                await context.SaveChangesAsync();

            if (!eventTriggered && logUpdate)
                context.InvokeSysEvent(SysEventType.IssueUpdated, userId, iss);

            return true;
        }

        public static async Task<Comment> AddCommentAsync(this ApplicationDbContext context, ICommentRegistration model, string userId, bool updateIssue = true)
        {
            var cmt = new Comment
            {
                Body = model.Body,
                Recipients = model.Recipients,
                ForTechsOnly = model.ForTechsOnly ?? false,
                IssueId = model.IssueId,
                IsSystem = model.IsSystem,
                UserId = userId,
            };

            await context.Comments.AddAsync(cmt);
            await context.SaveChangesAsync();

            if (updateIssue)
                await context.UpdateIssueAsync(model.IssueId
                    , userId
                    , model.CloseTicket ?? false
                    , save: true
                    , updatedForTechView: model.ForTechsOnly);

            return cmt;
        }

        public static async Task<Comment> AddSysCommentAsync(this ApplicationDbContext context, string body, int issueId)
          => await context.AddCommentAsync(new CommentRegistration
          {
              Body = body,
              IssueId = issueId,
              IsSystem = true,
          }, userId: null, updateIssue: false);

        #endregion

        #region AppUser Extensions

        public static string GetEmailAddress(this AppUser user) => EmailRecipient(user.Email, user.FullName(true));

        public static IQueryable<AppUser> Not(this IQueryable<AppUser> query, string userId) => query.Where(u => u.Id != userId);

        public static AppUser Find(this IQueryable<AppUser> query, string userId) => query.Where(u => u.Id == userId).SingleOrDefault();

        public static AppUser FindByUserName(this IQueryable<AppUser> query, string userName) => query.Where(u => u.UserName == userName).SingleOrDefault();

        /// <summary>
        /// Returns true if the user either owns the specified issue, or is a technician or an administrator; otherwise returns false.
        /// </summary>
        /// <param name="user">The user to test.</param>
        /// <param name="issue">The issue to test against.</param>
        /// <returns></returns>
        public static bool CanComment(this AppUser user, Issue issue) =>
          user != null && issue != null && (issue.UserId == user.Id || user.IsAdminOrTech());

        /// <summary>
        /// Returns true only if the specified issue has not been assigned and the user is an admin or a tech; otherwise, returns false.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="issue">The issue to check.</param>
        /// <returns></returns>
        public static bool CanTakeOver(this AppUser user, Issue issue) =>
          string.IsNullOrWhiteSpace(issue?.AssignedToUserId) && user.IsAdminOrTech();

        public static IQueryable<AppUser> Sort(this IQueryable<AppUser> q, int sortBy)
        {
            var desc = sortBy < 0;
            switch ((UserSorting)Math.Abs(sortBy))
            {
                case UserSorting.FirstName: q = desc ? q.OrderByDescending(u => u.FirstName) : q.OrderBy(u => u.FirstName); break;
                case UserSorting.LastName: q = desc ? q.OrderByDescending(u => u.LastName) : q.OrderBy(u => u.LastName); break;
                //case UserSorting.Company: query = desc ? query.OrderByDescending(e => e.Company.Name) : query.OrderBy(e => e.Company.Name); break;
                case UserSorting.LastSeen: q = desc ? q.OrderByDescending(u => u.LastSeen) : q.OrderBy(u => u.LastSeen); break;
                case UserSorting.SendEmail: q = desc ? q.OrderByDescending(u => u.SendEmail) : q.OrderBy(u => u.SendEmail); break;
                case UserSorting.TwoFactorAuth: q = desc ? q.OrderByDescending(u => u.TwoFactorEnabled) : q.OrderBy(u => u.TwoFactorEnabled); break;
                case UserSorting.Disabled: q = desc ? q.OrderByDescending(u => u.Disabled) : q.OrderBy(u => u.Disabled); break;
                case UserSorting.UserName:
                default:
                    q = desc ? q.OrderByDescending(u => u.UserName) : q.OrderBy(u => u.UserName);
                    break;
            }

            return q;
        }

        public static IQueryable<AppUser> Filter(this IQueryable<AppUser> q, UserFilter id)
        {
            switch (id)
            {
                case UserFilter.TechsOnly: q = q.Where(e => e.IsTech); break;
                case UserFilter.AdminsOnly: q = q.Where(e => e.IsAdministrator); break;
                case UserFilter.Disabled: q = q.Where(e => e.Disabled); break;
                case UserFilter.RegularOnly: q = q.Where(e => !e.IsTech && !e.IsAdministrator); break;
                case UserFilter.Managers: q = q.Where(e => e.IsManager); break;
                case UserFilter.All:
                default:
                    break;
            }

            return q;
        }

        public static IQueryable<AppUser> Search(this IQueryable<AppUser> q, string terms)
        {
            var arr = terms.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // not very handsome but gets some results
            q = from u in q
                where 
                u.FirstName.Contains(terms) | arr.Contains(u.FirstName) |
                u.LastName.Contains(terms) | arr.Contains(u.LastName) |
                u.UserName.Contains(terms) | arr.Contains(u.UserName) |
                u.Email.Contains(terms) | arr.Contains(u.Email) |
                //u.Notes.Contains(terms) | arr.Contains(u.Notes) |
                u.PhoneNumber.Contains(terms) | arr.Contains(u.PhoneNumber)
                select u;
            return q;
        }

        public static IQueryable<AppUser> Disabled(this IQueryable<AppUser> query) => query.Where(u => u.Disabled);

        public static IQueryable<AppUser> NotDisabled(this IQueryable<AppUser> query) => query.Where(u => !u.Disabled);

        public static IQueryable<AppUser> CanReceiveEmails(this IQueryable<AppUser> query, bool? confirmed = true)
        {
            query = query.Where(u => u.SendEmail);
            if (confirmed.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == confirmed.Value);
            }
            return query;
        }

        public static IQueryable<AppUser> EmailConfirmed(this IQueryable<AppUser> query) => query.Where(u => u.EmailConfirmed);

        public static IQueryable<AppUser> EmailNotConfirmed(this IQueryable<AppUser> query) => query.Where(u => !u.EmailConfirmed);

        public static IQueryable<AppUser> Admins(this IQueryable<AppUser> query) => query.Where(u => u.IsAdministrator);

        public static IQueryable<AppUser> Techs(this IQueryable<AppUser> query) => query.Where(u => u.IsTech);

        public static IQueryable<AppUser> Regular(this IQueryable<AppUser> query) => query.Where(u => !u.IsAdministrator && !u.IsTech);

        //public static IQueryable<AppUser> WithRole(this IQueryable<AppUser> query, string role)
        //  => query.Where(u => role == "admin" ? u.IsAdministrator : (role == "tech" ? u.IsTech : true));

        public static async Task<AppUser> FindUserAsync(this ApplicationDbContext context, string userId, string currentUserId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.Equals(userId, currentUserId))
            {
                return await context.Users.FindAsync(userId);
            }
            return await context.Users.FindAsync(currentUserId);
        }

        public static string UserType(this AppUser u)
          => u.IsAdministrator ? Administrator : (u.IsTech ? Technician : User);

        public static string UserRole(this AppUser u)
          => u.IsAdministrator ? "admin" : (u.IsTech ? "tech" : "user");

        public static object AsSnapshot(this AppUser u) => new
        {
            u.Id,
            u.UserName,
            u.Email,
            u.FirstName,
            u.LastName,
            LastSeen = GetDateString(u.LastSeen),
            SeenSince = TranslateDays(u.LastSeen),
            u.SendEmail,
            TwoFactor = u.TwoFactorEnabled,
            u.Disabled,
        };

        public static object AsDetail(this Employee e) => new
        {
            Id = e.UserId,
            Type = e.User.UserType(),
            Role = e.User.UserRole(),
            e.User.UserName,
            e.User.FirstName,
            e.User.LastName,
            e.User.Email,
            Phone = e.User.PhoneNumber,
            PhoneExtension = e.PhoneNumberExtension,
            CompanyName = e.Company?.Name,
            CompanyId = e.Company?.Id,
            e.DepartmentId,
            DepartmentName = e.Department?.Name,
            e.Location,
            LastSeen = GetDateString(e.User.LastSeen),
            SeenSince = TranslateTime(e.User.LastSeen),
            e.User.Greeting,
            e.User.Notes,
            e.User.SendEmail,
            e.User.SendNewTicketTechEmail,
            TwoFactor = e.User.TwoFactorEnabled,
            e.User.Disabled,
            e.User.PictureUrl,
            e.User.FacebookId,
            e.Locale,
            e.Gender,
            e.User.IsManager,
            e.User.IsTech,
            e.Signature,
            e.User.IPAddress,
            e.User.HostName,
            TicketsSubmitted = 0,
            TicketsHandled = 0,
            TicketsCreatedForOthers = 0,
        };

        public static async Task<bool> IsAdminOrTech(this ApplicationDbContext context, string userId)
          => (await context.Users.FindAsync(userId)).IsAdminOrTech();

        /// <summary>
        /// Checks whether the specified user is a technician (a user who can act upon issues), or an administrator.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsAdminOrTech(this AppUser user) => true == user?.IsAdministrator || true == user?.IsTech;

        #endregion

        public static async Task<Company> GetOrCreateCompanyAsync(this ApplicationDbContext context, string companyName)
        {
            companyName = companyName.Trim();
            var comp = await context.Companies.Where(c => c.Name == companyName).FirstOrDefaultAsync();
            if (comp == null)
            {
                comp = new Company { Name = companyName };
                await context.Companies.AddAsync(comp);
                await context.SaveChangesAsync();
            }
            return comp;
        }

        public static string Mode(this Category c)
          => c.ForTechsOnly ? "TechsOnly" : (c.ForSpecificUsers ? "SpecificUsers" : "Everyone");

        public static object AsDetail(this Category c) => new
        {
            c.SectionId,
            c.ForSpecificUsers,
            c.ForTechsOnly,
            c.FromAddress,
            c.FromAddressInReplyTo,
            c.FromName,
            c.Id,
            c.KbOnly,
            c.Name,
            c.Notes,
            c.OrderByNumber,
            Mode = c.Mode(),
            DifferentFrom = !string.IsNullOrWhiteSpace(c.FromAddress),
        };

        public static IQueryable<Category> QueryCategories(this ApplicationDbContext context, bool? isUserTechOrAdmin = null, bool unordered = false)
        {
            IQueryable<Category> query = context.Categories;
            if (isUserTechOrAdmin.HasValue)
            {
                if (!isUserTechOrAdmin.Value)
                {
                    // query for regular users only
                    query = query
                      .Where(c => !c.ForTechsOnly)
                      .Where(c => !c.KbOnly);
                }
            }
            if (!unordered) query = query.OrderBy(c => c.OrderByNumber).ThenBy(c => c.Name);
            return query;
        }

        /// <summary>
        /// Returns a detailed anonymous issue.
        /// </summary>
        /// <param name="i">The issue to derive from.</param>
        /// <param name="comments">The comments belonging to the specified issue.</param>
        /// <param name="assignee">The user who has been assigned the issue.</param>
        /// <param name="canTakeOver">Can the issue be assigned to the current user?</param>
        /// <param name="owned">Has the issue been created by the current user?</param>
        /// <param name="overTaken">Has the issue been assigned to the current user?</param>
        /// <param name="canComment">Can the current user add comments to the issue?</param>
        /// <returns></returns>
        public static object AsDetail(this Issue i, IEnumerable<Comment> comments, string assignee = null, bool canTakeOver = false, bool owned = false, bool overTaken = false, bool canComment = false) => new
        {
            i.Id,
            i.Subject,
            i.Body,
            i.UserId,
            i.StatusId,
            canTakeOver,
            overTaken,
            owned,
            canComment,
            Status = i.Status?.Name,
            Priority = PriorityName(i.Priority),
            From = i.User?.FullName(),
            assignee,
            AssigneeId = i.AssignedToUserId,
            Category = i.Category?.Name,
            IssueDate = i.IssueDate.ToString(FULL_DATE_TIME),
            IssuedSince = TranslateTime(i.IssueDate),
            LastUpdated = i.LastUpdated.ToString(FULL_DATE_TIME),
            UpdatedSince = TranslateTime(i.LastUpdated),
            DueOn = GetDateString(i.DueDate),
            DueSince = TranslateTime(i.DueDate),
            StartedOn = GetDateString(i.StartDate),
            StartedSince = TranslateTime(i.StartDate),
            ResolvedOn = GetDateString(i.ResolvedDate),
            ResolvedSince = TranslateTime(i.ResolvedDate),
            TimeSpent = i.TimeSpentInSeconds,
            Comments = comments.Select(c => c.AsDetail()),
        };

        public static object AsSnapshot(this Issue i) => new
        {
            i.Id,
            i.Subject,
            i.Body,
            i.StatusId,
            Status = i.Status?.Name,
            Category = i.Category?.Name,
            Priority = PriorityName(i.Priority),
            IssueDate = i.IssueDate.ToString(LONG_DATE_TIME),
            IssuedSince = TranslateTime(i.IssueDate),
            LastUpdated = i.LastUpdated.ToString(LONG_DATE_TIME),
            UpdatedSince = TranslateTime(i.LastUpdated),
            From = i.User?.FullName(),
            i.UserId,
        };

        public static object ForUpdate(this Issue i) => new
        {
            i.Id,
            i.Subject,
            i.Body,
        };

        public static object ForUpdate(this Comment c) => new
        {
            c.Id,
            c.Body,
            c.ForTechsOnly,
            c.IssueId,
        };

        /// <summary>
        /// Returns a detailed anonymous comment.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="dateFmt"></param>
        /// <returns></returns>
        public static object AsDetail(this Comment c, string dateFmt = FULL_DATE_TIME) => new
        {
            c.Id,
            c.Body,
            ForTechs = c.ForTechsOnly,
            System = c.IsSystem,
            Date = c.CommentDate.ToString(dateFmt),
            Since = TranslateTime(c.CommentDate),
            Author = c.User?.FullName(),
            AuthorId = c.User?.Id,
            AuthorEmail = c.User?.Email,
            AuthorIsTech = c.User?.IsAdminOrTech(),
            AuthorPicture = c.User?.PictureUrl,
            c.IssueId,
            issueSubject = c.Issue?.Subject,
            //file,
        };

        public static async Task<Employee> FindEmployeeAsync(this IQueryable<Employee> query, string userId)
          => await query.Include(e => e.User).Where(e => e.UserId == userId).SingleOrDefaultAsync();

        public static IQueryable<Issue> QueryIssues(this ApplicationDbContext context, string userId = null, int? id = null, bool withIncludes = true)
            => context.Issues.QueryIssues(userId, id, withIncludes);

        public static IQueryable<Issue> QueryIssues(this IQueryable<Issue> query, string userId = null, int? id = null, bool withIncludes = true)
        {
            if (withIncludes)
                query = query
                  .Include(c => c.User)
                  .Include(c => c.Category);

            if (id != null)
                query = query.Where(c => c.Id == id);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(c => c.UserId == userId);

            return query.Include(c => c.Status);
        }

        public static async Task<IQueryable<Issue>> QueryIssuesForAdminOrTechAsync(this ApplicationDbContext context, string userId, int? id = null, bool withIncludes = true) => context.QueryIssues(await context.IsAdminOrTech(userId) ? null : userId, id, withIncludes);

        public static IQueryable<Comment> QueryComments(this ApplicationDbContext context
            , int? id = null
            , string userId = null
            , bool excludeSystem = true)
            => context.Comments.QueryComments(id, userId, excludeSystem);

        public static IQueryable<Comment> QueryComments(this IQueryable<Comment> query
            , int? id = null
            , string userId = null
            , bool excludeSystem = true)
        {
            query = query.Include(c => c.Issue);

            if (excludeSystem)
                query = query.Where(c => !c.IsSystem);

            if (id != null)
                query = query.Where(c => c.Id == id);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(c => c.UserId == userId);

            return query;
        }

        public static IQueryable<Comment> QueryCommentsForIssue(this ApplicationDbContext context, int issueId)
            => context.QueryComments(excludeSystem: false).QueryCommentsForIssue(issueId);

        public static IQueryable<Comment> QueryCommentsForIssue(this IQueryable<Comment> query, int issueId)
          => query.Include(c => c.User).Where(c => c.IssueId == issueId).OrderBy(c => c.IsSystem).ThenByDescending(c => c.Id);

        public static async Task<List<Comment>> RecentComments(this ApplicationDbContext context, int issueId, int max = 3)
            => await context.QueryCommentsForIssue(issueId).RecentComments(issueId, max);

        public static async Task<List<Comment>> RecentComments(this IQueryable<Comment> query, int issueId, int max = 3)
          => await query.Skip(0).Take(max).ToListAsync();

        public static async Task<bool> CanTakeOverAsync(this ApplicationDbContext context, string userId, int issueId, Action<Issue> callback = null)
        {
            var issue = await context.QueryIssues().Where(i => i.Id == issueId).SingleOrDefaultAsync();
            if (issue != null && await context.CanTakeOverAsync(userId, issue))
            {
                callback?.Invoke(issue);
                return true;
            }
            return false;
        }

        public static async Task<bool> CanTakeOverAsync(this ApplicationDbContext context, string userId, Issue issue)
          => (await context.Users.FindAsync(userId)).CanTakeOver(issue);

        public static async Task<bool> CanCommentAsync(this ApplicationDbContext context, string userId, int issueId)
          => await context.CanCommentAsync(userId, await context.Issues.FindAsync(issueId));

        public static async Task<bool> CanCommentAsync(this ApplicationDbContext context, string userId, Issue issue)
          => (await context.Users.FindAsync(userId)).CanComment(issue);

        public static async Task<PagedList<Department>> GetDepartmentsAsync(this ApplicationDbContext context, int pageIndex, int pageSize)
            => await context.Departments.OrderBy(d => d.Name).GetPageAsync(pageIndex, pageSize);

        public static async Task<PagedList<Category>> GetCategoriesAsync(this ApplicationDbContext context, int pageIndex, int pageSize, bool? isUserTechOrAdmin = null, int? sortBy = null, string searchTerms = null)
        {
            var query = context.QueryCategories(isUserTechOrAdmin, unordered: sortBy.HasValue);
            if (sortBy.HasValue)
            {
                var sortVal = sortBy.Value;
                var desc = sortVal < 0;
                switch ((EntityListSorting)Math.Abs(sortVal))
                {
                    case EntityListSorting.Name:
                        query = desc ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name);
                        break;
                    case EntityListSorting.Id:
                        query = desc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id);
                        break;
                    default:
                        query = query.OrderBy(e => e.OrderByNumber).ThenBy(e => e.Name);
                        break;
                }
            }
            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                query = query.Search(searchTerms);
            }
            return await query.GetPageAsync(pageIndex, pageSize);
        }

        public static IQueryable<Category> Search(this IQueryable<Category> query, string searchTerms)
        {
            var arr = searchTerms.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            query = from q in query
                    where
    q.FromAddress.Contains(searchTerms) | arr.Contains(q.FromAddress) |
    q.FromName.Contains(searchTerms) | arr.Contains(q.FromName) |
    q.Name.Contains(searchTerms) | arr.Contains(q.Name) |
    q.Notes.Contains(searchTerms) | arr.Contains(q.Notes)
                    select q;

            return query;
        }

        public static async Task<IList<Category>> GetOrCreateCategoriesAsync(this ApplicationDbContext context, bool? isUserTechOrAdmin = null)
        {
            var items = await context.QueryCategories(isUserTechOrAdmin).ToListAsync();

            if (isUserTechOrAdmin == null && items.Count == 0)
            {
                lock (typeof(EntityExtensions))
                {
                    if (context.Categories.Count() == 0)
                    {
                        items = new List<Category>
                        {
                            new Category { Name = CategoryGeneralIssue },
                            new Category { Name = CategoryGeneralInquiry },
                            new Category { Name = CategoryGeneralInquirySupplyRequest },
                            new Category { Name = CategoryIPv4CompanyAddress },
                            new Category { Name = CategoryFeedback },
                            new Category { Name = CategoryVpnAccess },
                            new Category { Name = CategoryVsatIssue },
                            new Category { Name = CategoryInternetAccess },
                            new Category { Name = CategoryFirewallChangeRequest },
                            new Category { Name = CategoryRouterFirewall },
                            new Category { Name = CategoryCctvIssue },
                            new Category { Name = CategoryElfiqLlbIssue },
                            new Category { Name = CategoryIpPhoneIssue },
                            new Category { Name = CategoryIpPhoneIssueNewUser },
                            new Category { Name = CategoryEmergency },
                        };
                        context.Categories.AddRange(items);
                        context.SaveChanges();
                    }
                } // endlock
            }

            return items;
        }

        public static async Task<PagedList<Section>> GetSectionsAsync(this ApplicationDbContext context, int pageIndex, int pageSize)
            => await context.Sections
            .OrderBy(c => c.OrderByNumber)
            .ThenBy(c => c.Name)
            .GetPageAsync(pageIndex, pageSize);

        public static async Task<IList<Section>> GetOrCreateSectionsAsync(this ApplicationDbContext context)
        {
            var items = await context.Sections
              .OrderBy(c => c.OrderByNumber)
              .ThenBy(c => c.Name)
              .ToListAsync();

            if (items.Count == 0)
            {
                lock (typeof(EntityExtensions))
                {
                    if (context.Sections.Count() == 0)
                    {
                        items = new List<Section>
                        {
                          new Section { Name = SectionAccounting },
                          new Section { Name = SectionManagement },
                          new Section { Name = SectionIT },
                          new Section { Name = SectionLogistics },
                          new Section { Name = SectionSecretariat },
                        };
                        context.Sections.AddRange(items);
                        context.SaveChanges();
                    }
                }
            }

            return items;
        }

        public static async Task<IList<Status>> GetOrCreateStatusesAsync(this ApplicationDbContext context)
        {
            var items = await context.Statuses
              .OrderBy(c => c.Name)
              .ToListAsync();

            if (items.Count == 0)
            {
                lock (typeof(EntityExtensions))
                {
                    if (context.Statuses.Count() == 0)
                    {
                        items = new List<Status>
                        {
                          new Status { Name = StatusDeleted, ForTechsOnly = true },
                          new Status { Name = StatusNew, ForTechsOnly = true },
                          new Status { Name = StatusInProgress, ForTechsOnly = true },
                          new Status { Name = StatusClosed, ForTechsOnly = true },
                        };
                        context.Statuses.AddRange(items);
                        context.SaveChanges();
                    }
                }
            }
            return items;
        }

        public static IQueryable<Issue> Sort(this IQueryable<Issue> query, int sortBy)
        {
            var desc = sortBy < 0;
            switch ((IssueSorting)Math.Abs(sortBy))
            {
                case IssueSorting.Id: query = desc ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id); break;
                case IssueSorting.Priority: query = desc ? query.OrderByDescending(i => i.Priority) : query.OrderBy(i => i.Priority); break;
                case IssueSorting.IssueDate: query = desc ? query.OrderByDescending(i => i.IssueDate) : query.OrderBy(i => i.IssueDate); break;
                //case IssueSorting.CategoryFromAddress: query = desc ? query.OrderByDescending(i => i.Category.FromAddress) : query.OrderBy(i => i.Category.FromAddress); break;
                case IssueSorting.Tech: break;
                case IssueSorting.DueDate: query = desc ? query.OrderByDescending(i => i.DueDate) : query.OrderBy(i => i.DueDate); break;
                case IssueSorting.LastUpdated: query = desc ? query.OrderByDescending(i => i.LastUpdated) : query.OrderBy(i => i.LastUpdated); break;
                case IssueSorting.Company: break;
                case IssueSorting.StatusId: query = desc ? query.OrderByDescending(i => i.StatusId) : query.OrderBy(i => i.StatusId); break;
                case IssueSorting.Subject:
                default: query = desc ? query.OrderByDescending(i => i.Subject) : query.OrderBy(i => i.Subject); break;
            }
            return query;
        }

        public static AppSetting FindByName(this IQueryable<AppSetting> settings, string name)
          => settings.Where(s => s.Name == name).SingleOrDefault();

        public static async Task<AppSetting> FindByNameAsync(this IQueryable<AppSetting> settings, string name)
          => await settings.Where(s => s.Name == name).SingleOrDefaultAsync();

        public static AppSetting GetEmailSettings(this IQueryable<AppSetting> settings)
          => settings.FindByName("email-settings");

        public static async Task<AppSetting> GetEmailSettingsAsync(this IQueryable<AppSetting> settings)
          => await settings.FindByNameAsync("email-settings");

        /// <summary>
        /// Returns a subset of the specified query using the specified parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query to enumerate.</param>
        /// <param name="pageIndex">The zero-based page index.</param>
        /// <param name="pageSize">The maximum number of items to return. -1 to return all items.</param>
        /// <returns></returns>
        public static async Task<PagedList<T>> GetPageAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            var total = await query.CountAsync();
            var offset = pageSize == -1 ? 0 : pageIndex * pageSize;
            pageSize = pageSize == -1 ? total : pageSize;
            var items = await query.Skip(offset).Take(pageSize).ToArrayAsync();
            return new PagedList<T>(items, items.Length, pageIndex + 1, pageSize, total);
        }
    }
}
