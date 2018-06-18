using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HelpDeskCore.Data.Entities
{
    public class Comment : UserBase, ICloneable
    {
        public int Id { get; set; }
        [Required]
        public int IssueId { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.UtcNow;
        public string Body { get; set; }
        public bool ForTechsOnly { get; set; }
        public bool IsSystem { get; set; }
        public string Recipients { get; set; }
        public string EmailHeaders { get; set; }

        [JsonIgnore]
        public Issue Issue { get; set; }

        [JsonIgnore]
        public override AppUser User { get => base.User; set => base.User = value; }

        public Comment Clone() => (Comment)MemberwiseClone();

        object ICloneable.Clone() => Clone();
    }
}
