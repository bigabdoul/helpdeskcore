using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.ViewModels.Emails
{
  public class ResetPasswordViewModel
  {
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Token { get; set; }
    public string BaseUrl { get; set; }
    public string AppName { get; set; }
  }
}
