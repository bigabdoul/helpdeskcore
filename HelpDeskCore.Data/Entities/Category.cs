using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HelpDeskCore.Data.Entities
{
  public class Category : ICloneable, Shared.Logging.ILogItem
    {
    public Category()
    {
      Issues = new HashSet<Issue>();
    }

    public int Id { get; set; }
    [StringLength(250)]
    public string Name { get; set; }
    public int? SectionId { get; set; }
    public bool ForTechsOnly { get; set; }
    public bool ForSpecificUsers { get; set; }
    public int OrderByNumber { get; set; }
    [StringLength(255)]
    public string FromAddress { get; set; }
    public bool KbOnly { get; set; }
    public bool FromAddressInReplyTo { get; set; }
    public string Notes { get; set; }
    [StringLength(255)]
    public string FromName { get; set; }

    [JsonIgnore]
    public Section Section { get; set; }
    [JsonIgnore]
    public ICollection<Issue> Issues { get; set; }

    public Category Clone() => (Category)MemberwiseClone();

    object ICloneable.Clone() => Clone();
  }
}
