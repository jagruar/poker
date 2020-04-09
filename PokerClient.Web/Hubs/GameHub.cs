using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PokerClient.Web.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private string PlayerId => Context.User.Claims.FirstOrDefault(c => c.Type == "player-id")?.Value;

        public async override Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, PlayerId);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            // TODO Disconnect/boot player
            await base.OnDisconnectedAsync(exception);
        }
    }
}
