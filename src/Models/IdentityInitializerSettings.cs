namespace HelpDeskCore.Models
{
  /// <summary>
  /// Exposes properties used during initialization of identity-related database entities.
  /// </summary>
  public class IdentityInitializerSettings
  {
    public string AdminId { get; set; }
    public string AdminEmail { get; set; }
    public string AdminUserName { get; set; }
    public string AdminPassword { get; set; }
  }
}
