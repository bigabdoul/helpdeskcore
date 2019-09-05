using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
  public class FileAttachment : UserBase
  {
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int CommentId { get; set; }
    [Required]
    [StringLength(255)]
    public string FileName { get; set; }
    public byte[] FileData { get; set; }
    public byte[] FileHash { get; set; }
    public int FileSize { get; set; }
    [StringLength(255)]
    public string GoogleDriveUrl { get; set; }
    [StringLength(500)]
    public string DropboxUrl { get; set; }
    public bool HiddenFromKB { get; set; }
    public bool HiddenFromTickets { get; set; }

    public Issue Issue { get; set; }
    public Comment Comment { get; set; }
  }
}
