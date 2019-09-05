using System;
using System.Linq;
using System.Threading.Tasks;
using CoreRepository;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Shared.Logging;
using Microsoft.AspNetCore.Identity;
using static Newtonsoft.Json.JsonConvert;

namespace HelpDeskCore.Data.Logging
{
    public sealed class SysEventLogger : SysEventLoggerBase
    {
        AppUser _user;
        readonly IRepository<AppUser> _userRepository;
        readonly Repository.ISysEventLogRepository _eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SysEventLogger"/> class using the specified parameters.
        /// </summary>
        /// <param name="userRepository">The repository for interacting with the <see cref="IdentityUser"/> entity.</param>
        /// <param name="eventRepository">The repository for interacting with the <see cref="SysEventLogEntry"/> entity.</param>
        public SysEventLogger(IRepository<AppUser> userRepository, Repository.ISysEventLogRepository eventRepository)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Asynchronously adds an event entry to the log.
        /// </summary>
        /// <param name="args">The arguments of the event to log.</param>
        /// <returns></returns>
        public override async Task LogAsync(SysEventArgs args)
        {
            var type = args.EventType;
            var user = args.User;
            var usr = await AsUser(user, type);
            args.User = usr;

            if (type == SysEventType.LoginSuccess)
            {
                try
                {
                    // update last seen
                    var u = await _userRepository.GetAsync(q => q.Where(e => e.Id == usr.Id).SingleOrDefault());
                    u.LastSeen = DateTime.UtcNow;
                    u.HostName = args.Data?.ToString();
                    await _userRepository.SaveChangesAsync();
                }
                catch (Exception)
                {
                }
            }

            await base.LogAsync(args);
        }

        /// <summary>
        /// Asynchronously adds an event log entry using the underlying repository.
        /// </summary>
        /// <param name="type">The type of the event to log.</param>
        /// <param name="descr">The description of the event.</param>
        /// <param name="state">A custom object related to the event, which will be serialized.</param>
        /// <returns></returns>
        protected override async Task AddEventEntryAsync(SysEventType type, string descr, object state = null)
        {
            try
            {
                string strData = null;

                if (state is string)
                    strData = (string)state;
                else if (state != null)
                    try { strData = SerializeObject(state); } catch { }

                _eventRepository.Add(new SysEventLog
                {
                    UserId = _user.Id,
                    Description = descr,
                    EventType = type.ToString(),
                    ObjectState = strData,
                });

                await _eventRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        protected override Task AddSysCommentAsync(string body, int? parentId = null)
            => _eventRepository.AddSysCommentAsync(body, parentId.Value);

        protected override string GetUserName(SysEventType type)
        {
            switch (type)
            {
                case SysEventType.LoginFailure:
                case SysEventType.LoginSuccess:
                case SysEventType.Logout: return _user?.UserName;
            }
            return _user?.FullName();
        }

        async Task<IdentityUser> AsUser(object user, SysEventType type)
        {
            if (user == null) return null;
            _user = _user ?? user as AppUser;

            if (_user != null) return _user;

            if (type == SysEventType.LoginFailure)
            {
                _user = await _userRepository.GetAsync(q => q.Where(u => u.UserName == user.ToString()).SingleOrDefault());
            }
            else
            {
                _user = await _userRepository.GetAsync(q => q.Where(u => u.Id == user.ToString()).SingleOrDefault());
            }
            return _user;
        }
    }
}
