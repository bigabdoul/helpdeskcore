namespace HelpDeskCore.Shared.Logging
{
    /// <summary>
    /// Enumerates the types of system events.
    /// </summary>
    public enum SysEventType
    {
        /// <summary>
        /// Unspecified event.
        /// </summary>
        Unspecified = -1,

        /// <summary>
        /// Login attempt failed.
        /// </summary>
        LoginFailure = 1,

        /// <summary>
        /// Login attempt was successful.
        /// </summary>
        LoginSuccess,

        /// <summary>
        /// User logged out.
        /// </summary>
        Logout,

        /// <summary>
        /// Issue created.
        /// </summary>
        IssueCreated,

        /// <summary>
        /// Issue assigned to someone.
        /// </summary>
        IssueAssigned,

        /// <summary>
        /// Issue updated.
        /// </summary>
        IssueUpdated,

        /// <summary>
        /// Issue closed.
        /// </summary>
        IssueClosed,

        /// <summary>
        /// Issue re-opened.
        /// </summary>
        IssueReopened,

        /// <summary>
        /// Issue deleted.
        /// </summary>
        IssueDeleted,

        /// <summary>
        /// User registered themselves through the web UI.
        /// </summary>
        UserRegistered,

        /// <summary>
        /// User created by an admin.
        /// </summary>
        UserCreated,

        /// <summary>
        /// User updated by an admin or themselves.
        /// </summary>
        UserUpdated,

        /// <summary>
        /// User deleted by an admin.
        /// </summary>
        UserDeleted,

        /// <summary>
        /// User imported by an admin.
        /// </summary>
        UserImported,

        /// <summary>
        /// User password was changed.
        /// </summary>
        UserPasswordChanged,

        /// <summary>
        /// Category created by an admin.
        /// </summary>
        CategoryCreated,

        /// <summary>
        /// Category updated.
        /// </summary>
        CategoryUpdated,

        /// <summary>
        /// Category updated.
        /// </summary>
        CategoryDeleted,

        /// <summary>
        /// Email configuration settings updated.
        /// </summary>
        EmailConfigUpdated,

        /// <summary>
        /// Sending an e-mail has failed.
        /// </summary>
        EmailSendFailed,

        /// <summary>
        /// Sending an e-mail succeeded.
        /// </summary>
        EmailSendSuccess,
    }

    /// <summary>
    /// Enumerates the categories of system events.
    /// </summary>
    public enum SysEventCategory
    {
        /// <summary>
        /// Unknown category.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Login/logout category.
        /// </summary>
        Login = 1,

        /// <summary>
        /// Issues.
        /// </summary>
        Issue,

        /// <summary>
        /// Users.
        /// </summary>
        User,

        /// <summary>
        /// Issue categories.
        /// </summary>
        Category,

        /// <summary>
        /// Emails.
        /// </summary>
        Email,
    }
}
