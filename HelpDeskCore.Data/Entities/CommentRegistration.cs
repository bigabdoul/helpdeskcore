namespace HelpDeskCore.Data.Entities
{
    internal class CommentRegistration : ICommentRegistration
    {
        public bool? CloseTicket { get; set; }
        public string Body { get; set; }
        public bool? ForTechsOnly { get; set; }
        public int IssueId { get; set; }
        public bool IsSystem { get; set; }
        public string Recipients { get; set; }
    }
}
