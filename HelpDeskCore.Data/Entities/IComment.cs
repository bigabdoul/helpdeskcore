namespace HelpDeskCore.Data.Entities
{
    public interface ICommentRegistration
    {
        string Body { get; set; }
        bool? ForTechsOnly { get; set; }
        int IssueId { get; set; }
        bool IsSystem { get; set; }
        string Recipients { get; set; }
        bool? CloseTicket { get; set; }
    }
}