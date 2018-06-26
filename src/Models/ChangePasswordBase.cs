using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models
{
  public class ChangePasswordBase
  {
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
  }
}
