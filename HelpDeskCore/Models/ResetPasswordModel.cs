using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models
{
  public class ResetPasswordModel : ChangePasswordBase
  {
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Token { get; set; }
  }
}
