using System.ComponentModel.DataAnnotations;
using HelpDeskCore.Data.Entities;

namespace HelpDeskCore.ViewModels
{
  public class TicketBaseViewModel
  {
    [Required]
    public string Subject { get; set; }
    //[Required]
    public string Body { get; set; }
  }

  public class TicketRegistrationViewModel : TicketBaseViewModel
  {
    public int CategoryId { get; set; }
    public short Priority { get; set; }
  }

  public class TicketModificationViewModel : TicketBaseViewModel
  {
    [Required]
    public int Id { get; set; }
  }

  public class CommentBaseViewModel
  {
    [Required]
    public string Body { get; set; }
    public string Recipients { get; set; }
    public bool? ForTechsOnly { get; set; }
    public bool? CloseTicket { get; set; }
  }

  public class CommentRegistrationViewModel : CommentBaseViewModel, ICommentRegistration
  {
    [Required]
    public int IssueId { get; set; }
    public bool IsSystem { get; set; }
  }

  public class CommentModificationViewModel : CommentBaseViewModel
  {
    [Required]
    public int Id { get; set; }
  }

  public class SetIssueStatusModel
  {
    [Required]
    public int IssueId { get; set; }
    [Required]
    public TicketStatus Status { get; set; }
  }
}
