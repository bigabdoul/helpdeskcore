using MailkitTools;

namespace HelpDeskCore.ViewModels.Emails
{
  public class SmtpSettings : IEmailClientConfiguration
  {
    public string ServerAddress { get; set; }
    public int ServerPort { get; set; } = 25;
    public bool? RequiresAuth { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool? UseSsl { get; set; }

    string IEmailClientConfiguration.Host { get => ServerAddress; set => ServerAddress=value; }
    int IEmailClientConfiguration.Port { get => ServerPort; set => ServerPort = value; }
    bool IEmailClientConfiguration.UseSsl { get => UseSsl ?? false; set => UseSsl = value; }
    string IEmailClientConfiguration.UserName { get => UserName; set => UserName = value; }
    string IEmailClientConfiguration.Password { get => Password; set => Password = value; }
    bool IEmailClientConfiguration.RequiresAuth { get => RequiresAuth ?? false; set => RequiresAuth = value; }
  }
}
