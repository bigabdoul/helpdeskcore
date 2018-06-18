using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Data.Entities
{
  public class AppSetting : ICloneable
  {
    public int Id { get; set; }
    [Required]
    [StringLength(500)]
    public string Name { get; set; }
    public string Value { get; set; }

    public AppSetting Clone() => new AppSetting { Id = Id, Name = Name, Value = Value };

    object ICloneable.Clone() => Clone();
  }
}
