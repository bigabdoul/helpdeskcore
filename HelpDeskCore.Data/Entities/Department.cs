using System.ComponentModel.DataAnnotations;

namespace HelpDeskCore.Data.Entities
{
    public class Department : Shared.Logging.ILogItem
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
    }
}
