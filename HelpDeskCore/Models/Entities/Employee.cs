using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace HelpDeskCore.Models.Entities
{
  public partial class Employee : UserBase, ICloneable
  {
    public int Id { get; set; }
    [StringLength(1000)]
    public string Location { get; set; }
    [StringLength(10)]
    public string Locale { get; set; }
    [StringLength(1)]
    public string Gender { get; set; }
    [StringLength(50)]
    public string PhoneNumberExtension { get; set; }
    public int? CompanyId { get; set; }
    public int? DepartmentId { get; set; }
    [StringLength(2000)]
    public string Signature { get; set; }
    [JsonIgnore]
    public Department Department { get; set; }
    [JsonIgnore]
    public Company Company { get; set; }

    public Employee Clone() => (Employee)MemberwiseClone();

    object ICloneable.Clone() => Clone();
  }
}
