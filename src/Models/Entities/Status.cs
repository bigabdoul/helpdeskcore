using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
  public partial class Status : ICloneable
  {
    public Status()
    {
      Issues = new HashSet<Issue>();
    }

    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    [StringLength(50)]
    public string ButtonCaption { get; set; }
    public bool ForTechsOnly { get; set; }
    public bool StopTimeSpent { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public ICollection<Issue> Issues { get; set; }

    public Status Clone() => (Status)MemberwiseClone();
    object ICloneable.Clone() => Clone();
  }
}
