namespace HelpDeskCore.Services.Imports
{
  public interface IUserImported
  {
    string Company { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Location { get; set; }
    string Notes { get; set; }
    string Password { get; set; }
    string PhoneNumber { get; set; }
    string Title { get; set; }
    string UserName { get; set; }
  }
}
