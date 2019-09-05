namespace HelpDeskCore.ViewModels.Emails
{
  public class EmailSettingsViewModel
  {
    public IncomingMailSettings Incoming { get; set; } = new IncomingMailSettings();

    public EmailNotificationSettings Notifications { get; set; } = new EmailNotificationSettings
    {
      // default values
      Enabled = true, NotifyTechs = true, NotifyAllAdmins = true, TicketConfirmationNotification = true, TicketClosedNotification = true,
    };

    public OutgoingEmailSettings Outgoing { get; set; } = new OutgoingEmailSettings();
    public SmtpSettings Smtp { get; set; } = new SmtpSettings();
    public EmailTemplateSettings Templates { get; set; } = new EmailTemplateSettings();
  }
}
