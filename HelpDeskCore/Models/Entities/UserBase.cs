using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
  public abstract class UserBase
  {
    public string UserId { get; set; }
    public virtual AppUser User { get; set; }
  }
}
