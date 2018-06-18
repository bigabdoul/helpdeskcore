using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Logging;
using HelpDeskCore.Helpers;
using HelpDeskCore.Models;
using HelpDeskCore.Services.Emails;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static HelpDeskCore.Data.Extensions.EntityExtensions;
using static HelpDeskCore.Resources.Strings;

namespace HelpDeskCore.Controllers
{
  [Authorize(Policy = "ApiUser")]
  [Route("api/[controller]")]
  public class TicketsController : HelpDeskControllerBase
  {
    private static readonly object[] _sortingList = new[]
      {
        new { Id = 1, Name = TicketSortingId },
        new { Id = 2, Name = TicketSortingPriority },
        new { Id = 3, Name = TicketSortingDate },
        new { Id = 4, Name = TicketSortingSubject },
        new { Id = 5, Name = TicketSortingFrom },
        new { Id = 6, Name = TicketSortingTech },
        new { Id = 7, Name = TicketSortingDeadline },
        new { Id = 8, Name = TicketSortingLastModified },
        new { Id = 9, Name = TicketSortingCompany },
        new { Id = 10, Name = TicketSortingStatus },
      };

    readonly IIssueEmailProducer _emailProducer;

    public TicketsController(ApplicationDbContext appDbContext
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , ISysEventLogger sysLogger
      , IIssueEmailProducer emailProducer)
      : base(appDbContext, identitySettings, httpContextAccessor, sysLogger)
    {
      _emailProducer = emailProducer;
    }

    // GET api/tickets/index
    [HttpGet("index")]
    public async Task<IActionResult> Index([FromQuery] PaginationModel model)
      => await GetPagedIssues(await Db.QueryIssuesForAdminOrTechAsync(GetUserId(), withIncludes: false), model);

    // GET api/tickets/foruser
    [HttpGet("foruser")]
    public async Task<IActionResult> ForUser([FromQuery] PaginationModel model)
    {
      if (await CanAccess(model.UserId))
      {
        return await GetPagedIssues(Db.QueryIssues(model.UserId, withIncludes: false), model);
      }
      return BadRequest();
    }

    // GET api/tickets/history/20727a3f-d7d6-42d8-800d-8cfa5237ad69
    [HttpGet("history/{id}")]
    public async Task<IActionResult> History(string id)
    {
      if (await CanAccess(id))
      {
        try
        {
          var p1 = await Db.QueryIssues(id, withIncludes: false).Sort(-1).GetPageAsync(0, -1);
          var p2 = await Db.QueryComments(userId: id).OrderByDescending(c => c.Id).GetPageAsync(0, -1);

          return new OkObjectResult(new
          {
            tickets = p1.Select(i => i.AsSnapshot()),
            comments = p2.Select(c => c.AsDetail())
          });
        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex);
        }
      }
      return BadRequest();
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
      var userId = GetUserId();
      var issue = await Db.GetIssueAsync(id, userId);
      if (issue == null) return NotFound();
      return new OkObjectResult(await AnonymousIssue(issue, userId));
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
      var issue = await Db.GetIssueAsync(id, GetUserId());
      if (issue == null) return NotFound();
      return new OkObjectResult(issue.ForUpdate());
    }

    [HttpGet("edit-comment/{id}")]
    public async Task<IActionResult> EditComment(int id)
    {
      var comment = await GetCommentAsync(id, GetUserId());
      if (comment == null) return NotFound();
      return new OkObjectResult(comment.ForUpdate());
    }

    // POST api/tickets
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]TicketRegistrationViewModel model)
    {
      var issue = new Issue
      {
        Body = model.Body,
        Subject = model.Subject,
        CategoryId = model.CategoryId,
        UpdatedByUser = true,
        StatusId = (int)TicketStatus.New,
        UserId = GetUserId(),
        Priority = model.Priority,
      };
      
      await Db.Issues.AddAsync(issue);
      await Db.SaveChangesAsync();
      await EventLogger.LogAsync(SysEventType.IssueCreated, issue.UserId, issue);

      return Ok();
    }

    // PUT api/tickets
    [HttpPut]
    public async Task<IActionResult> Put([FromBody]TicketModificationViewModel model)
    {
      var issue = await Db.GetIssueAsync(model.Id, GetUserId(), false);

      if (issue == null)
        return NotFound();

      // clone the issue before altering it
      var oldIssue = issue.Clone();

      issue.Body = model.Body;
      issue.Subject = model.Subject;
      var success = await Db.UpdateIssueAsync(issue, CurrentUserId, newStatus: TicketStatus.Unchanged, save: true);

      if (success)
      {
        var cmt = await Db.AddSysCommentAsync(TicketUpdated, issue.Id);
        await Db.AddFileAttachementAsync(oldIssue, cmt.Id, CurrentUserId);
        await EventLogger.LogAsync(SysEventType.IssueUpdated, CurrentUserId, issue, oldIssue);
      }

      return new OkObjectResult(success ? TicketUpdated : TicketNotUpdated);
    }

    [HttpPost("takeover/{id}")]
    public async Task<IActionResult> TakeOver(int id)
    {
      Issue issue = null;

      if (!await Db.CanTakeOverAsync(GetUserId(), id, iss => issue = iss))
        return BadRequest();

      issue.AssignedToUserId = CurrentUserId;

      if (await Db.UpdateIssueAsync(issue, CurrentUserId, save: true, logUpdate: false))
      {
        await EventLogger.LogAsync(SysEventType.IssueAssigned, CurrentUserId, issue);
      }

      return Ok();
    }

    [HttpPost("setStatus")]
    public async Task<IActionResult> SetStatus([FromBody] SetIssueStatusModel model)
    {
      var issue = await Db.GetIssueAsync(model.IssueId, GetUserId(), withIncludes: false);

      if (issue == null)
        return NotFound();

      await Db.UpdateIssueAsync(issue, CurrentUserId, save: true, newStatus: model.Status);

      return Ok();
    }

    [HttpPost("comment")]
    public async Task<IActionResult> PostComment([FromBody]CommentRegistrationViewModel model)
    {
      var result = await CannotCommentAsync(model.IssueId);

      if (result != null)
        return result;

      var cmt = await Db.AddCommentAsync(model, GetUserId());
      return new OkObjectResult(cmt.AsDetail());
    }

    [HttpPut("comment")]
    public async Task<IActionResult> UpdateComment([FromBody]CommentModificationViewModel model)
    {
      var cmt = await GetCommentAsync(model.Id, GetUserId());
      if (cmt == null) return NotFound();
      var result = await CannotCommentAsync(cmt.IssueId);

      if (result != null)
        return result;

      cmt.Body = model.Body;
      cmt.ForTechsOnly = model.ForTechsOnly ?? false;

      var success = await Db.UpdateIssueAsync(cmt.IssueId, CurrentUserId, model.CloseTicket ?? false, save: true, logUpdate: false);
      return Ok();
    }

    [HttpGet("categories")]
    public async Task<IActionResult> Categories()
    {
      var items = await Db.GetOrCreateCategoriesAsync(await Db.IsAdminOrTech(GetUserId()));
      return new OkObjectResult(items.Select(c => new
      {
        c.Id,
        c.Name,
      }).ToArray());
    }

    [HttpGet("sections")]
    public async Task<IActionResult> Sections()
    {
      var items = await Db.GetOrCreateSectionsAsync();
      return new OkObjectResult(items.Select(c => new
      {
        c.Id,
        c.Name,
      }).ToArray());
    }

    [HttpGet("sortings")]
    public IActionResult Sortings()
    {
      return new OkObjectResult(_sortingList);
    }

    #region event logging

    protected override Task AfterEventLoggedAsync(object sender, SysEventArgs e)
    {
      //if (e.GetEventCategory() == SysEventCategory.Issue)
      {
        try
        {
          var config = EmailDispatcher.Instance.EmailSettings;

          //if (config?.Notifications.Enabled ?? false)
          {
            // clone entities to avoid trouble when the dbcontext that created them goes out of scope (disposed)
            e.User = ((AppUser)e.User).Clone();
            e.ObjectState = config;

            return _emailProducer.ProcessAsync(new SysEventArgs<Issue>(e));
          }
        }
        catch (Exception ex)
        {
          Trace.WriteLine(ex);
        }
      }

      return Task.CompletedTask;
    }

    #endregion

    #region helpers

    //private async Task<bool> UpdateIssueAsync(int issueId, string userId, bool closeTicket = false, bool save = false, TicketStatus? newStatus = null, bool? updatedForTechView = null)
    //  => await DbContext.UpdateIssueAsync(await GetIssueAsync(issueId, userId, false), userId, closeTicket, save, newStatus, updatedForTechView);

    //private async Task<Issue> GetIssueAsync(int id, string userId = null, bool withIncludes = true)
    //  // an admin or tech can access all tickets
    //  => await DbContext.QueryIssues(await IsAdminOrTech() ? null : userId, id, withIncludes).SingleOrDefaultAsync();

    async Task<Comment> GetCommentAsync(int id, string userId, bool excludeSystem = true)
      => await Db.QueryComments(id, await Db.IsAdminOrTech(userId) ? null : userId, excludeSystem).SingleOrDefaultAsync();

    /// <summary>
    /// Converts the specified issue to an anonymous object that can be safely serialized.
    /// </summary>
    /// <param name="issue">The issue to convert.</param>
    /// <param name="userId">The identifier of the currently logged-in user.</param>
    /// <returns></returns>
    async Task<object> AnonymousIssue(Issue issue, string userId)
    {
      AppUser assignee = null;
      var assigneeId = issue.AssignedToUserId;

      if (!string.IsNullOrEmpty(assigneeId))
        assignee = await Db.Users.FindAsync(assigneeId);

      var owned = issue.UserId == userId;
      var overTaken = assigneeId == userId;

      // get the current user but make sure to avoid another round-trip to the database
      var user = assigneeId == userId
        ? assignee // assignee is the same as the current user
        : await Db.Users.FindAsync(userId);

      var canTake = user.CanTakeOver(issue);
      var canComment = user.CanComment(issue);
      var comments = await Db.QueryCommentsForIssue(issue.Id).ToArrayAsync();

      return issue.AsDetail(comments, assignee?.FullName(), canTake, owned, overTaken, canComment);
    }
    
    async Task<IActionResult> CannotCommentAsync(int issueId)
    {
      if (!await Db.CanCommentAsync(GetUserId(), issueId))
        return BadRequest(ModelState.AddError("", CannotCommentIssue));

      return null;
    }

    async Task<IActionResult> GetPagedIssues(IQueryable<Issue> query, PaginationModel model)
    {
      var sortBy = model.SortBy ?? 0;
      var page = model.Page ?? 0;
      var size = model.Size ?? 10;
      if (!string.IsNullOrWhiteSpace(model.Query))
      {
        query = query.Search(model.Query);
      }
      var p = await query.Sort(sortBy).AsNoTracking().GetPageAsync(page, size);
      return new OkObjectResult(new { p.TotalCount, p.PageCount, items = p.Select(i => i.AsSnapshot()).ToArray() });
    }

    async Task<bool> CanAccess(string userId)
      => await Db.IsAdminOrTech(GetUserId()) || CurrentUserId == userId;

    #endregion
  }
}
