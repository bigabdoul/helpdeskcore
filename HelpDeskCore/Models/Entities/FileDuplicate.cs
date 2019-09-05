using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
    public class FileDuplicate
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public int CommentId { get; set; }
    }
}
