using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Data.Entities
{
  public abstract class UserBase
  {
    public string UserId { get; set; }
    public virtual AppUser User { get; set; }
  }
}
