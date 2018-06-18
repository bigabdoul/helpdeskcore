using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HelpDeskCore.Data;
using HelpDeskCore.Models;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static System.Threading.Thread;

namespace HelpDeskCore.Controllers
{
  /// <summary>
  /// Represents the application's base controller.
  /// </summary>
  public abstract class HelpDeskControllerBase : Controller
  {
    #region fields

    /// <summary>
    /// The current application user profile.
    /// </summary>
    protected Employee CurrentEmployee;

    /// <summary>
    /// The current application user.
    /// </summary>
    protected AppUser CurrentUser;

    /// <summary>
    /// The current application user identifier.
    /// </summary>
    protected string CurrentUserId;

    private readonly ClaimsPrincipal _caller;

    /// <summary>
    /// The application database context.
    /// </summary>
    protected readonly ApplicationDbContext Db;

    protected readonly IdentityInitializerSettings IdentitySettings;

    protected readonly ISysEventLogger EventLogger;

    static readonly CultureInfo FrenchCulture = new CultureInfo("fr");

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpDeskControllerBase"/> class using the specified parameters.
    /// </summary>
    /// <param name="dbContext">The database context used.</param>
    /// <param name="httpContextAccessor">An object used to access the current user's claims principal.</param>
    protected HelpDeskControllerBase(ApplicationDbContext dbContext, IOptions<IdentityInitializerSettings> identitySettings, IHttpContextAccessor httpContextAccessor, ISysEventLogger sysLogger)
    {
      Db = dbContext;
      EventLogger = sysLogger;
      IdentitySettings = identitySettings.Value;
      _caller = httpContextAccessor.HttpContext.User;

      CurrentThread.CurrentCulture = FrenchCulture;
      CurrentThread.CurrentUICulture = FrenchCulture;

      Db.SysEvent += Db_SysEvent;
      EventLogger.Logged += AfterEventLoggedAsync;
    }

    /// <summary>
    /// Returns the current user's identifier from the calling principal's claims collection.
    /// </summary>
    /// <returns></returns>
    protected virtual string GetUserId() => CurrentUserId ?? (CurrentUserId = _caller.Claims.SingleOrDefault(c => c.Type == "id")?.Value);

    /// <summary>
    /// Returns the specified or current employee.
    /// </summary>
    /// <param name="userId">The identifier of the user to find.</param>
    /// <param name="details">true to include details such as company, and department info; otherwise, false.</param>
    /// <returns></returns>
    protected virtual async Task<Employee> FindEmployeeAsync(string userId = null, bool details = false)
    {
      if (!string.IsNullOrWhiteSpace(userId) && !string.Equals(userId, GetUserId()))
      {
        // just search for the specified user
        return await findEmployee(userId);
      }

      // search for the current user, and store eventually in 'CurrentUser' field
      var emp = await findEmployee(GetUserId());

      if (CurrentUser == null && emp?.User != null)
        CurrentUser = emp.User;

      return emp;

      async Task<Employee> findEmployee(string uid)
      {
        var query = Db.Employees.Include(e => e.User).Where(e => e.UserId == uid);

        if (details)
          query = query.Include(e => e.Company).Include(e => e.Department);

        return await query.SingleOrDefaultAsync();
      }
    }

    /// <summary>
    /// Returns the specified or current application user.
    /// </summary>
    /// <param name="userId">The identifier of the user to find.</param>
    /// <returns></returns>
    protected virtual async Task<AppUser> FindUserAsync(string userId = null)
    {
      if (!string.IsNullOrWhiteSpace(userId) && !string.Equals(userId, GetUserId()))
      {
        return await Db.Users.FindAsync(userId);
      }
      return CurrentUser ?? (CurrentUser = await Db.Users.FindAsync(GetUserId())); ;
    }

    /// <summary>
    /// Checks whether the specified or current user is a built-in administrator.
    /// </summary>
    /// <param name="userId">The identifier of the user to find.</param>
    /// <returns></returns>
    protected async Task<bool> IsBuiltInAdmin(string userId = null)
      => string.Equals((await FindUserAsync(userId))?.Id, IdentitySettings.AdminId, System.StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the current user is an administrator.
    /// </summary>
    /// <param name="userId">The identifier of the user to find.</param>
    /// <returns></returns>
    protected virtual async Task<bool> IsAdmin(string userId = null) => (await FindUserAsync(userId))?.IsAdministrator == true;

    /// <summary>
    /// Method invoked after a system event has been logged. Since it does nothing, classes that inherit <see cref="HelpDeskControllerBase"/> should override it to do something meaningful.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event data.</param>
    /// <returns></returns>
    protected virtual Task AfterEventLoggedAsync(object sender, SysEventArgs e)
      => Task.CompletedTask;

    /// <summary>
    /// Events logged on behalf of the <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event data.</param>
    /// <returns></returns>
    private Task Db_SysEvent(object sender, SysEventArgs e) => EventLogger.LogAsync(e);
  }
}
