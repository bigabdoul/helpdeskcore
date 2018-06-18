namespace HelpDeskCore.ViewModels.Emails
{
  public class IncomingMailSettings
  {
    public int? NewTicketsDefaultCategoryId { get; set; }
    public bool? AcceptEmailsFromUnregisteredUsers { get; set; }
    public bool? AddEmailsFromCcAndToForTicketSubscribers { get; set; }
    public bool? ExtractOriginalSenderFromForwardedEmailsCreateTicket { get; set; }
  }
}
