using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Services.Imports
{
  /// <summary>
  /// Represents an imported user.
  /// </summary>
  public class UserImported : IUserImported
  {
    [Display(Order = 0)] public string UserName { get; set; }
    [Display(Order = 1)] public string Password { get; set; }
    [Display(Order = 2)] public string Email { get; set; }
    [Display(Order = 3)] public string FirstName { get; set; }
    [Display(Order = 4)] public string LastName { get; set; }
    [Display(Order = 5)] public string Company { get; set; }
    [Display(Order = 6)] public string PhoneNumber { get; set; }
    [Display(Order = 7)] public string Location { get; set; }
    [Display(Order = 8)] public string Title { get; set; }
    [Display(Order = 9)] public string Notes { get; set; }
  }
}
