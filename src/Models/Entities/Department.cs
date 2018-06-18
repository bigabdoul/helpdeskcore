using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
    }
}
