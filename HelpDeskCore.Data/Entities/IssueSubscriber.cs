using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Data.Entities
{
  public class IssueSubscriber
  {
    public int IssueId { get; set; }
    [StringLength(128)]
    public string UserId { get; set; }
    public Issue Issue { get; set; }
    public AppUser User { get; set; }
  }
}
