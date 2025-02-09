using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicMateAPI.Services;

namespace MusicMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoiceControlController : ControllerBase
    {
        private readonly IVoiceControlService _voiceControlService;

        public VoiceControlController(IVoiceControlService voiceControlService)
        {
            _voiceControlService = voiceControlService;
        }

        [HttpPost("interpret")]
        public async Task<IActionResult> InterpretVoiceCommand(
            [FromBody] VoiceCommandRequest request
        )
        {
            var result = await _voiceControlService.InterpretCommandAsync(request.Command);
            return Ok(new { Result = result });
        }
    }

    public class VoiceCommandRequest
    {
        public string Command { get; set; }
    }
}
