using System;
using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Data.Extensions;
using Microsoft.Extensions.Options;

namespace HelpDeskCore.Data.Repository
{
    public class SysEventLogRepository : Repository<SysEventLog>, ISysEventLogRepository
    {
        public SysEventLogRepository(IOptions<DbAccessOptions> options) : base(options)
        {
        }

        public virtual async Task AddSysCommentAsync(string body, int issueId)
        {
            if( Context is ApplicationDbContext ctx)
            {
                await ctx.AddSysCommentAsync(body, issueId);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
