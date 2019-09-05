namespace HelpDeskCore.Services.Emails
{
  /// <summary>
  /// A marker interface that specifies the data contract required for a
  /// service that produces issue-related email notifications for subscribers.
  /// </summary>
  public interface IIssueEmailProducer : Shared.Messaging.IMessageProducer
  {
  }
}
