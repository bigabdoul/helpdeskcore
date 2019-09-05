using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }
    }
}
