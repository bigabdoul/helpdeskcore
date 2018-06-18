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

            services.Configure<DbAccessOptions>(options => options.CreateContext = createContext);

            services.AddTransient<IRepository<AppSetting>, Repository<AppSetting>>();
            services.AddTransient<IRepository<AppUser>, Repository<AppUser>>();
            services.AddTransient<IRepository<Issue>, Repository<Issue>>();
            services.AddTransient<IRepository<IssueSubscriber>, Repository<IssueSubscriber>>();
            services.AddTransient<IRepository<Comment>, Repository<Comment>>();
            services.AddTransient<IRepository<Category>, Repository<Category>>();
            services.AddTransient<IRepository<Company>, Repository<Company>>();
            services.AddTransient<IRepository<Department>, Repository<Department>>();
            services.AddTransient<IRepository<Employee>, Repository<Employee>>();
            services.AddTransient<IRepository<FileAttachment>, Repository<FileAttachment>>();
            services.AddTransient<IRepository<FileDuplicate>, Repository<FileDuplicate>>();
            services.AddTransient<IRepository<Section>, Repository<Section>>();
            services.AddTransient<IRepository<Status>, Repository<Status>>();
            services.AddTransient<IRepository<SysEventLog>, Repository<SysEventLog>>();
            services.AddTransient<IRepository<UserAvatar>, Repository<UserAvatar>>();
            services.AddTransient<ISysEventLogRepository, SysEventLogRepository>();
            services.AddTransient<ISysEventLogger, Logging.SysEventLogger>();

            return services;
        }
    }
}
