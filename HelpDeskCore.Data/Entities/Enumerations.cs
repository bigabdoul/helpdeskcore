namespace HelpDeskCore.Data.Entities
{
    public enum TicketStatus
    {
        Unchanged = 0,
        Deleted = 1,
        New = 2,
        InProgress = 3,
        Resolved = 4
    }

    public enum TicketPriority : short
    {
        Low = -1,
        Normal,
        High,
        Critical,
    }

    public enum IssueSorting
    {
        Id = 1,
        Priority = 2,
        IssueDate = 3,
        Subject = 4,
        //CategoryFromAddress = 5,
        Tech = 6,
        DueDate = 7,
        LastUpdated = 8,
        Company = 9,
        StatusId = 10,
    }

    public enum UserSorting
    {
        UserName = 1,
        FirstName = 2,
        LastName = 3,
        Company = 4,
        LastSeen = 5,
        SendEmail = 6,
        TwoFactorAuth = 7,
        Disabled = 8,
    }

    public enum UserFilter
    {
        All = 1,
        TechsOnly = 2,
        AdminsOnly = 3,
        Disabled = 4,
        RegularOnly = 5,
        Managers = 6,
    }

    public enum EntityListSorting
    {
        Name = 1,
        Id = 2,
    }
}
