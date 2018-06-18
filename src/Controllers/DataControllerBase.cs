using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpDeskCore.Data;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared.Logging;
using HelpDeskCore.Data.Extensions;
using HelpDeskCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace HelpDeskCore.Controllers
{
  /// <summary>
  /// Used to lazy-initialize the database with seed data.
  /// </summary>
  public abstract class DataControllerBase : HelpDeskControllerBase
  {
    #region static

    private static IdentityInitializer _initializer;
    private static object _initializerLock;
    private static bool _isInitialized;

    static DataControllerBase()
    {
      _initializerLock = new object();
    }

    #endregion

    /// <summary>
    /// The user manager.
    /// </summary>
    protected readonly UserManager<AppUser> UserManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataControllerBase"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="dbContext">The application database context.</param>
    /// <param name="identitySettings"></param>
    protected DataControllerBase(UserManager<AppUser> userManager
      , ApplicationDbContext dbContext
      , IOptions<IdentityInitializerSettings> identitySettings
      , IHttpContextAccessor httpContextAccessor
      , ISysEventLogger sysLog) : base(dbContext, identitySettings, httpContextAccessor, sysLog)
    {
      UserManager = userManager;
    }

    /// <summary>
    /// Called before the action method is called in order to lazy-initialize the database.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      try
      {
        LazyInitializer.EnsureInitialized(
          ref _initializer,
          ref _isInitialized,
          ref _initializerLock,
          () => new IdentityInitializer(UserManager, Db, IdentitySettings)
        );
      }
      catch (System.Reflection.TargetInvocationException)
      {
      }
      base.OnActionExecuting(context);
    }
    
    private class IdentityInitializer
    {
      internal IdentityInitializer(UserManager<AppUser> userManager, ApplicationDbContext context, IdentityInitializerSettings settings)
      {
        if (!context.Users.Any())
        {
          CreateOrUpdateUsersAsync(userManager, context, settings).Wait();
        }
        if (!context.Categories.Any())
        {
          context.GetOrCreateCategoriesAsync().Wait();
        }
        if (!context.Statuses.Any())
        {
          context.GetOrCreateStatusesAsync().Wait();
        }
        if (!context.Sections.Any())
        {
          context.GetOrCreateSectionsAsync().Wait();
        }
      }

      async Task CreateOrUpdateUsersAsync(UserManager<AppUser> userManager, ApplicationDbContext context, IdentityInitializerSettings settings)
      {
        var userName = settings.AdminUserName;
        var appUser = context.Users.SingleOrDefault(u => u.UserName == userName);
        var userId = !string.IsNullOrWhiteSpace(settings.AdminId) ? settings.AdminId : System.Guid.NewGuid().ToString().ToLower();

        if (appUser == null)
        {
          appUser = new AppUser
          {
            Id = userId,
            Email = userName,
            IsAdministrator = true,
            UserName = userName,
          };

          var result = await userManager.CreateAsync(appUser, settings.AdminPassword);

          if (result.Succeeded)
          {
            await context.Employees.AddAsync(new Employee { UserId = appUser.Id });
            await context.SaveChangesAsync();
          }
        }
      }
    }
  }
}
