using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicMateAPI.Data;
using MusicMateAPI.Models;

namespace MusicMateAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly OAuthService _oAuthService;
        private readonly IAuthService _authService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly AppDbContext _context;

        public TestAuthController(
            OAuthService oAuthService,
            IAuthService authService,
            ITokenBlacklistService tokenBlacklistService,
            AppDbContext context
        )
        {
            _oAuthService = oAuthService;
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
            _context = context;
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

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _tokenBlacklistService.BlacklistToken(token);
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(
                new
                {
                    user.Id,
                    user.Email,
                    // Add other user properties as needed
                }
            );
        }

        [HttpPost("update-preferences")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPreferences(
            [FromBody] UserPreferences preferences
        )
        {
            var userId = User
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var userPreferences = await _context.UserPreferences.FirstOrDefaultAsync(p =>
                p.UserId == Guid.Parse(userId) && p.PreferenceKey == preferences.PreferenceKey
            );

            if (userPreferences == null)
            {
                userPreferences = new UserPreferences
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    PreferenceKey = preferences.PreferenceKey,
                    PreferenceValue = preferences.PreferenceValue,
                };
                _context.UserPreferences.Add(userPreferences);
            }
            else
            {
                userPreferences.PreferenceValue = preferences.PreferenceValue;
                _context.UserPreferences.Update(userPreferences);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Preferences updated successfully" });
        }
    }
}
