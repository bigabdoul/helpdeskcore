using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HelpDeskCore.Services.Notifications
{
  [Authorize(Policy = "ApiUser")]
  public class NotificationHub : Hub<Shared.Messaging.INotificationHub>
  {
    public NotificationHub()
    {
    }

    public override Task OnConnectedAsync()
    {
      return base.OnConnectedAsync();
    }
  }
}
