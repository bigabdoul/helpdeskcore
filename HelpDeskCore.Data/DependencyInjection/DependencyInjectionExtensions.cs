using System;
using CoreRepository;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Repository;
using HelpDeskCore.Shared.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HelpDeskCore.Data.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds repositories for the supported entities.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="createContext">A callback function that creates a new instance of the <see cref="DbContext"/> 
        /// class that supports the entities. See <see cref="ApplicationDbContext"/> for the supported entities.
        /// </param>
        /// <returns>A reference to the specified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHelpDeskCoreRepositories(this IServiceCollection services, Func<DbContext> createContext)
        {
            if (createContext == null) throw new ArgumentNullException(nameof(createContext));

            return services
                .Configure<DbAccessOptions>(options => options.CreateContext = createContext)
                .AddTransient<IRepository<AppSetting>, Repository<AppSetting>>()
                .AddTransient<IRepository<AppUser>, Repository<AppUser>>()
                .AddTransient<IRepository<Issue>, Repository<Issue>>()
                .AddTransient<IRepository<IssueSubscriber>, Repository<IssueSubscriber>>()
                .AddTransient<IRepository<Comment>, Repository<Comment>>()
                .AddTransient<IRepository<Category>, Repository<Category>>()
                .AddTransient<IRepository<Company>, Repository<Company>>()
                .AddTransient<IRepository<Department>, Repository<Department>>()
                .AddTransient<IRepository<Employee>, Repository<Employee>>()
                .AddTransient<IRepository<FileAttachment>, Repository<FileAttachment>>()
                .AddTransient<IRepository<FileDuplicate>, Repository<FileDuplicate>>()
                .AddTransient<IRepository<Section>, Repository<Section>>()
                .AddTransient<IRepository<Status>, Repository<Status>>()
                .AddTransient<IRepository<SysEventLog>, Repository<SysEventLog>>()
                .AddTransient<IRepository<UserAvatar>, Repository<UserAvatar>>()
                .AddTransient<ISysEventLogRepository, SysEventLogRepository>()
                .AddTransient<ISysEventLogger, Logging.SysEventLogger>()
                .AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
