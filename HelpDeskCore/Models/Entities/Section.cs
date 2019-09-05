using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Models.Entities
{
    public partial class Section
    {
        public Section()
        {
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
        public int OrderByNumber { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
