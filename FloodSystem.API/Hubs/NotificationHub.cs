using Microsoft.AspNetCore.SignalR;

namespace FloodSystem.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinAllGroup()
            => await Groups.AddToGroupAsync(Context.ConnectionId, "All");

        public async Task JoinAdminGroup()
            => await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");

        public async Task JoinUserGroup(string userId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}