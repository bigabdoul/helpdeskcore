namespace HelpDeskCore.Models
{
  public class PaginationModel
  {
    public int? Page { get; set; }
    public int? Size { get; set; }
    public int? SortBy { get; set; }
    public string Query { get; set; }
    public string UserId { get; set; }
    public Data.Entities.UserFilter? Column { get; set; }
  }
}
