using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{
    private readonly OAuthService _oAuthService;
    private readonly IAuthService _authService;

    public TestAuthController(OAuthService oAuthService, IAuthService authService)
    {
        _oAuthService = oAuthService;
        _authService = authService;
    }

    [HttpGet("test-google")]
    public async Task<IActionResult> TestGoogle(string code)
    {
        var token = await _oAuthService.HandleGoogleAccessTokenAsync(code);
        return Ok(new { Token = token });
    }

    [HttpGet("test-spotify")]
    public async Task<IActionResult> TestSpotify(string code)
    {
        var token = await _oAuthService.HandleSpotifyAccessTokenAsync(code);
        return Ok(new { Token = token });
    }

    [HttpPost("test-jwt")]
    public async Task<IActionResult> TestJwt([FromBody] LoginRequest request)
    {
        var token = await _authService.AuthenticateUserAsync(request.Email, request.Password);
        return Ok(new { Token = token });
    }
}
