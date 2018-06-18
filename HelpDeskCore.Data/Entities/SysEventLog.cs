using System.ComponentModel.DataAnnotations.Schema;
using HelpDeskCore.Shared.Logging;

namespace HelpDeskCore.Data.Entities
{
    [Table("SysEventLog")]
    public class SysEventLog : SysEventLogEntry
    {
        public AppUser User { get; set; }
    }
}
