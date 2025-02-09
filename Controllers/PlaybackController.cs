using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MusicMateAPI.Hubs;

namespace MusicMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaybackController : ControllerBase
    {
        private readonly IHubContext<PlaybackHub> _hubContext;

        public PlaybackController(IHubContext<PlaybackHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncPlayback([FromBody] PlaybackSyncRequest request)
        {
            await _hubContext.Clients.All.SendAsync(
                "ReceivePlaybackSync",
                request.User,
                request.Action,
                request.Timestamp
            );
            return Ok(new { Message = "Playback sync sent successfully" });
        }
    }

    public class PlaybackSyncRequest
    {
        public string User { get; set; }
        public string Action { get; set; }
        public int Timestamp { get; set; }
    }
}
