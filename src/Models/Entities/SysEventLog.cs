using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskCore.Models.Entities
{
  [Table("SysEventLog")]
  public class SysEventLog
  {
    public int Id { get; set; }
    [Required]
    [StringLength(128)]
    public virtual string UserId { get; set; }
    [StringLength(500)]
    public virtual string Description { get; set; }
    [StringLength(50)]
    public virtual string EventType { get; set; }
    public virtual string ObjectState { get; set; }
    public virtual DateTime Date { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; }
  }

  [Table("EmailLog")]
  public class EmailLog : SysEventLog
  {
    [StringLength(128)]
    public override string UserId { get => base.UserId; set => base.UserId = value; }

    [Required]
    [StringLength(128)]
    public string MessageId { get; set; }
  }
}
