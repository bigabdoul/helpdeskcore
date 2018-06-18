using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared;
using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskCore.Data
{
    /// <summary>
    /// Base class for the Entity Framework database context used for the application.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        readonly string _connectionString;

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified parameter.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="ApplicationDbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion

        #region Database sets

        /// <summary>
        /// Gets or sets the categories data set.
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the comments data set.
        /// </summary>
        public virtual DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// Gets or sets the companies data set.
        /// </summary>
        public virtual DbSet<Company> Companies { get; set; }

        /// <summary>
        /// Gets or sets the departments data set.
        /// </summary>
        public virtual DbSet<Department> Departments { get; set; }

        /// <summary>
        /// Gets or sets the employees data set.
        /// </summary>
        public virtual DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Gets or sets the file attachments data set.
        /// </summary>
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }

        /// <summary>
        /// Gets or sets the file duplicates data set.
        /// </summary>
        public virtual DbSet<FileDuplicate> FileDuplicates { get; set; }

        /// <summary>
        /// Gets or sets the issues data set.
        /// </summary>
        public virtual DbSet<Issue> Issues { get; set; }

        /// <summary>
        /// Gets or sets the sections data set.
        /// </summary>
        public virtual DbSet<Section> Sections { get; set; }

        /// <summary>
        /// Gets or sets the statuses data set.
        /// </summary>
        public virtual DbSet<Status> Statuses { get; set; }

        /// <summary>
        /// Gets or sets the user avatars data set.
        /// </summary>
        public virtual DbSet<UserAvatar> UserAvatars { get; set; }

        /// <summary>
        /// Gets or sets the application settings data set.
        /// </summary>
        public virtual DbSet<AppSetting> AppSettings { get; set; }

        /// <summary>
        /// Gets or sets the system events log data set.
        /// </summary>
        public virtual DbSet<SysEventLog> Events { get; set; }

        /// <summary>
        /// Gets or sets the issue subscribers data set.
        /// </summary>
        public virtual DbSet<IssueSubscriber> Subscribers { get; set; }

        #endregion

        #region overrides

        /// <summary>
        /// Configures the schema for the entity framework.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().HasKey(e => e.Id);
            builder.Entity<Issue>().HasKey(e => e.Id);

            builder.Entity<IssueSubscriber>()
              .HasKey(si => new { si.IssueId, si.UserId });

            builder.Entity<IssueSubscriber>()
                .HasOne(si => si.Issue)
                .WithMany(i => i.IssueSubscribers)
                .HasForeignKey(si => si.IssueId);

            builder.Entity<IssueSubscriber>()
                .HasOne(si => si.User)
                .WithMany(u => u.IssueSubscribers)
                .HasForeignKey(si => si.UserId);
        }

        /// <summary>
        /// Configures the database (and other options) to be used for this context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        #endregion
        
        #region events

        /// <summary>
        /// Event fired when a system-wide event occurs.
        /// </summary>
        public event AsyncEventHandler<SysEventArgs> SysEvent;

        /// <summary>
        /// Event fired by the <see cref="ApplicationDbContext"/> itself when it
        /// performs internal operations not directly accessible to the caller.
        /// </summary>
        public static event AsyncEventHandler<SysEventArgs> Notify;

        #endregion
        
        /// <summary>
        /// Invokes the <see cref="SysEvent"/> event.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        /// <param name="user">The user who caused the event.</param>
        /// <param name="data">The event data.</param>
        /// <param name="previousObjectState">The previous state of the event data.</param>
        /// <returns></returns>
        public bool InvokeSysEvent(SysEventType type, object user, object data = null, object previousObjectState = null)
          => null != SysEvent?.Invoke(this, new SysEventArgs(type, user, data, previousObjectState));

        /// <summary>
        /// Attempts to invoke the <see cref="Notify"/> event.
        /// </summary>
        /// <param name="args">The event data.</param>
        internal static void OnNotify(SysEventArgs args)
        {
            try
            {
                Notify?.Invoke(typeof(ApplicationDbContext), args);
            }
            catch
            {
            }
        }
    }
}
