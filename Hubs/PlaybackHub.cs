using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MusicMateAPI.Hubs
{
    public class PlaybackHub : Hub
    {
        public async Task SyncPlayback(string user, string action, int timestamp)
        {
            await Clients.All.SendAsync("ReceivePlaybackSync", user, action, timestamp);
        }
    }
}
