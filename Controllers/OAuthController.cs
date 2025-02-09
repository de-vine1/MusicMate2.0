using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OAuthController : ControllerBase
{
    private readonly OAuthService _oAuthService;

    public OAuthController(OAuthService oAuthService)
    {
        _oAuthService = oAuthService;
    }

    [HttpGet("signin-google")]
    public async Task<IActionResult> SignInGoogle(string code)
    {
        var accessToken = await _oAuthService.GetGoogleAccessTokenAsync(code);
        // Handle the access token (e.g., create a session, generate a JWT, etc.)
        return Ok(new { AccessToken = accessToken });
    }

    [HttpGet("signin-spotify")]
    public async Task<IActionResult> SignInSpotify(string code)
    {
        var accessToken = await _oAuthService.GetSpotifyAccessTokenAsync(code);
        // Handle the access token (e.g., create a session, generate a JWT, etc.)
        return Ok(new { AccessToken = accessToken });
    }
}
