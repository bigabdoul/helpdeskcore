using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.ViewModels
{
  public class UserDetailViewModel
  {
    [Required]
    [StringLength(128)]
    public string Id { get; set; }
    [StringLength(256)]
    public string UserName { get; set; }
    [StringLength(100)]
    public string FirstName { get; set; }
    [StringLength(100)]
    public string LastName { get; set; }
    [StringLength(100)]
    public string Email { get; set; }
    [StringLength(50)]
    public string Phone { get; set; }
    [StringLength(20)]
    public string PhoneExtension { get; set; }
    [StringLength(200)]
    public string CompanyName { get; set; }
    public int? CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    [StringLength(200)]
    public string DepartmentName { get; set; }
    [StringLength(100)]
    public string Location { get; set; }
    [StringLength(100)]
    public string Greeting { get; set; }
    [StringLength(4000)]
    public string Notes { get; set; }
    public bool SendEmail { get; set; }
    public bool TwoFactor { get; set; }
    public bool Disabled { get; set; }
    [StringLength(255)]
    public string PictureUrl { get; set; }
    public long? FacebookId { get; set; }
    [StringLength(10)]
    public string Locale { get; set; }
    [StringLength(1)]
    public string Gender { get; set; }
    public bool IsManager { get; set; }
    [StringLength(2000)]
    public string Signature { get; set; }
    public bool IsTech { get; set; }
    public bool SendNewTicketTechEmail { get; set; }
    public string Role { get; set; }
  }
}
