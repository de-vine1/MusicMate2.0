using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicMateAPI.Services;

namespace MusicMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StreamingController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;

        public StreamingController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("stream/{songId}")]
        public async Task<IActionResult> StreamSong(string songId)
        {
            var songUrl = await _spotifyService.GetSongUrlAsync(songId);
            if (string.IsNullOrEmpty(songUrl))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = await _spotifyService.GetSongStreamAsync(songUrl))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "audio/mpeg", enableRangeProcessing: true);
        }
    }
}
