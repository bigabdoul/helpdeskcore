using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HelpDeskCore.Models.Entities
{
  public class Issue : UserBase, ICloneable
  {
    public Issue()
    {
      Comments = new HashSet<Comment>();
      IssueSubscribers = new HashSet<IssueSubscriber>();
    }

    public int Id { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    [StringLength(128)]
    public string AssignedToUserId { get; set; }
    [StringLength(1000)]
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool UpdatedByUser { get; set; }
    public bool UpdatedByPerformer { get; set; }
    public bool UpdatedForTechView { get; set; }
    public int StatusId { get; set; }
    public short Priority { get; set; }
    public int TimeSpentInSeconds { get; set; }
    public DateTime? DueDate { get; set; }

    [JsonIgnore]
    public Category Category { get; set; }
    [JsonIgnore]
    public Status Status { get; set; }
    [JsonIgnore]
    public ICollection<Comment> Comments { get; set; }
    [JsonIgnore]
    public ICollection<IssueSubscriber> IssueSubscribers { get; set; }

    /// <summary>
    /// Creates a shallow copy of the current issue.
    /// </summary>
    /// <returns></returns>
    public Issue Clone()
    {
      var i = (Issue)MemberwiseClone();
      i.User = i.User?.Clone();
      i.Category = i.Category?.Clone();
      i.Status = i.Status?.Clone();
      return i;
    }

    object ICloneable.Clone() => Clone();
  }
}
