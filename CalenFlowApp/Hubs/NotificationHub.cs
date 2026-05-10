using Microsoft.AspNetCore.SignalR;

namespace CalenFlowApp.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinGroup(string userEmail)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userEmail);
        }
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.Group(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
