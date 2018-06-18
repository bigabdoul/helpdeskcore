using System.Threading;
using System.Threading.Tasks;
using HelpDeskCore.Shared.Logging;

namespace HelpDeskCore.Shared.Messaging
{
    /// <summary>
    /// Specifies the data contract required for a service that produces system event-related notifications.
    /// </summary>
    public interface IMessageProducer
    {
        /// <summary>
        /// Asynchronously produces system event-related notifications according to the specified <see cref="SysEventArgs.EventType"/>.
        /// </summary>
        /// <param name="args">The event arguments.</param>
        /// <param name="cancellationToken">The token used to cancel an ongoing async operation.</param>
        /// <returns></returns>
        Task ProcessAsync(SysEventArgs args, CancellationToken cancellationToken = default(CancellationToken));
    }
}
