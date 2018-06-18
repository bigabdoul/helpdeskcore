namespace HelpDeskCore.ViewModels.Emails
{
  public class OutgoingEmailSettings
  {
    public string From { get; set; }
    public string FromName { get; set; }
    public bool? UseFromNameForAll { get; set; }
    public string ReplyTo { get; set; }

    public string FromDisplay { get => $"{FromName}<{From}>"; }
  }
}
