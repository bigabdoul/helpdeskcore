using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.ViewModels
{
  public class CategoryDetailViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Name { get; set; }

    public string Notes { get; set; }
    public int? SectionId { get; set; }

    /// <summary>
    /// Use a "from" address different from notification emails in this category.
    /// </summary>
    public bool? DifferentFrom { get; set; }

    [StringLength(255)]
    public string FromName { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string FromAddress { get; set; }

    public bool? FromAddressInReplyTo { get; set; }
    public bool KbOnly { get; set; }
    public string Mode { get; set; }
  }
}
