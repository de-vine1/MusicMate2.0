using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicMateAPI.Models;
using MusicMateAPI.Services;

namespace MusicMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfflineController : ControllerBase
    {
        private readonly IOfflineService _offlineService;

        public OfflineController(IOfflineService offlineService)
        {
            _offlineService = offlineService;
        }

        [HttpGet("songs")]
        public async Task<IActionResult> GetOfflineSongs()
        {
            var songs = await _offlineService.GetOfflineSongsAsync();
            return Ok(songs);
        }

        [HttpPost("cache")]
        public async Task<IActionResult> CacheSong([FromBody] CacheSongRequest request)
        {
            await _offlineService.CacheSongAsync(request.SongId);
            return Ok(new { Message = "Song cached successfully" });
        }
    }

    public class CacheSongRequest
    {
        public string SongId { get; set; }
    }
}
