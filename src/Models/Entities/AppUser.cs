
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskCore.Models.Entities
{
  // Add profile data for application users by adding properties to this class
  public class AppUser : IdentityUser, ICloneable
  {
    public AppUser()
    {
      IssueSubscribers = new HashSet<IssueSubscriber>();
    }
    
    [StringLength(255)]
    public string FirstName { get; set; }
    [StringLength(255)]
    public string LastName { get; set; }
    [StringLength(500)]
    public string Greeting { get; set; }
    public long? FacebookId { get; set; }
    [StringLength(255)]
    public string PictureUrl { get; set; }
    public bool IsAdministrator { get; set; }
    public bool IsManager { get; set; }
    public bool IsTech { get; set; }
    public bool SendNewTicketTechEmail { get; set; }
    [StringLength(4000)]
    public string Notes { get; set; }
    public bool SendEmail { get; set; }
    public DateTime? LastSeen { get; set; }
    [StringLength(50)]
    public string IPAddress { get; set; }
    [StringLength(255)]
    public string HostName { get; set; }
    public bool Disabled { get; set; }
    [Newtonsoft.Json.JsonIgnore]
    public ICollection<IssueSubscriber> IssueSubscribers { get; set; }

    public override string ToString() => UserName;

    public AppUser Clone() => (AppUser)MemberwiseClone();

    object ICloneable.Clone() => Clone();

    public string GetGreeting(string defaultValue = null)
    {
      var greet = Greeting;
      if (!string.IsNullOrWhiteSpace(greet))
      {
        greet = new System.Text.StringBuilder(greet)
          .Replace("#Prenom#", FirstName)
          .Replace("#Nom#", LastName)
          .Replace("#FirstName#", FirstName)
          .Replace("#LastName#", LastName)
          .ToString();
      }
      else if(!string.IsNullOrWhiteSpace(defaultValue))
      {
        greet = defaultValue;
      }
      else
      {
        greet = "Bonjour" + FirstOrLastName(", ", "!", " !");
      }
      return greet;
    }

    public string FullName(bool excludeUserName = false)
    {
      var name = $"{FirstName} {LastName}".Trim();
      if (!excludeUserName && string.IsNullOrEmpty(name))
        name = UserName;
      return name;
    }

    public string FirstOrLastName(string prefix = null, string suffix = null, string defaultValue = null)
    {
      if (!string.IsNullOrWhiteSpace(FirstName))
        return $"{prefix}{FirstName}{suffix}";
      else if (!string.IsNullOrWhiteSpace(LastName))
        return $"{prefix}{LastName}{suffix}";
      return defaultValue ?? string.Empty;
    }
  }
}
