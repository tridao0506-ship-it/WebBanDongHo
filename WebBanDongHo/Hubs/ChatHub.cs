using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace WebBanDongHo.Hubs
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext?.Request.Query["username"].FirstOrDefault();
            if (!string.IsNullOrEmpty(username))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, username);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext?.Request.Query["username"].FirstOrDefault();
            if (!string.IsNullOrEmpty(username))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, username);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string fromUser, string toUser, string message)
        {
            await Clients.Group(toUser).SendAsync("ReceiveMessage", fromUser, message);
        }
    }
}