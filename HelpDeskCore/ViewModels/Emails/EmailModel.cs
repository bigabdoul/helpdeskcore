using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.ViewModels.Emails
{
  public class EmailModel
  {
    public string Subject { get; set; }
    public string Body { get; set; }
    public string From { get; set; }
    [Required]
    public string To { get; set; }
  }
}
