using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models
{
  public class ChangePasswordModel : ChangePasswordBase
  {
    [Required]
    public string UserId { get; set; }

    [DataType(DataType.Password)]
    public string OldPassword { get; set; }
  }
}
