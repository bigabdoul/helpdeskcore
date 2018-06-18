using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data.Entities;

namespace HelpDeskCore.Data.Repository
{
    public interface ISysEventLogRepository : IRepository<SysEventLog>
    {
        Task AddSysCommentAsync(string body, int issueId);
    }
}
