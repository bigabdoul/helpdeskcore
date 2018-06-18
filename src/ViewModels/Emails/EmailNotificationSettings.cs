namespace HelpDeskCore.ViewModels.Emails
{
  /// <summary>
  /// Encapsulates e-mail notifications settings.
  /// </summary>
  /// <remarks>Each property describes how the app SHOULD behave.</remarks>
  public class EmailNotificationSettings
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotificationSettings"/> class.
    /// </summary>
    public EmailNotificationSettings()
    {
    }

    /// <summary>
    /// When this option is on, Helpdesk Core will notify users when their tickets are updated,
    /// notify the help-desk team of the new tickets and new updates on the existing tickets.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// When this option is on, ticket submitters will get email notifications when their ticket
    /// is created in the help desk. They will only get this notification, if they submit the
    /// ticket via email, not via the web-interface.
    /// </summary>
    public bool? TicketConfirmationNotification { get; set; }

    /// <summary>
    /// When this option is on, ticket submitters will get notifications when their tickets are closed.
    /// </summary>
    public bool? TicketClosedNotification { get; set; }

    /// <summary>
    /// Notifies helpdesk administrators about all created tickets in every category.
    /// </summary>
    public bool? NotifyAllAdmins { get; set; }

    /// <summary>
    /// Technicians will get notifications about the new tickets created in the categories they have
    /// access to. When a ticket is MOVED to a new category - this will also generate a notification
    /// to those technicians who haven't seen it in the old category.
    /// </summary>
    public bool? NotifyTechs { get; set; }

    /// <summary>
    /// Sends a notification to all technicians when a customer updates a ticket in their categories.
    /// No matter if techs are assigned to the ticket or not.
    /// </summary>
    public bool? NotifyAllTechsOnCustomerUpdate { get; set; }

    /// <summary>
    /// Notifies other technicians when someone takes over a ticket in their categories to prevent collisions.
    /// </summary>
    public bool? NotifyAllTechsOnTechTakeOver { get; set; }

    /// <summary>
    /// if you want to remove the files attached to a ticket from email notifications (for instance, for
    /// HIPAA compliance) - uncheck this box. This will also disable access to inline images attached to
    /// tickets from within email software.
    /// </summary>
    public bool? IncludeAttachmentsInOutgoing { get; set; }

    /// <summary>
    /// <para>
    /// By default we send "autologin" links - links users can click and login instantly without the need to
    /// enter a password - to ticket submitters in outgoing notifications. We send them only to regular users,
    /// not to techs or admins. These links are also time-limited to three days.
    /// </para>
    ///
    /// This is very convenient, but can be a potential security breach.When a users forwards his helpdesk
    /// notification to someone else, the recipient of the forwarded email can click the autologin link to
    /// login under the original user credentials.
    /// </summary>
    public bool? SendAutoLoginLinks { get; set; }
  }
}
