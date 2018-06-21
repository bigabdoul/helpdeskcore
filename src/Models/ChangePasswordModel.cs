using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models
{
  public class ChangePasswordModel
  {
    [Required]
    public string UserId { get; set; }

    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; }
  }
}
