namespace HelpDeskCore.Models
{
  public class PaginationModelBase
  {
    public int? Page { get; set; }
    public int? Size { get; set; }
    public int? SortBy { get; set; }
    public string Query { get; set; }
    public string UserId { get; set; }
  }

  public class PaginationModel : PaginationModelBase
  {
    public Data.Entities.UserFilter? Column { get; set; }
  }

  public class TicketPaginationModel : PaginationModelBase
  {
    public Data.Entities.TicketFilter? Column { get; set; }
  }
}
