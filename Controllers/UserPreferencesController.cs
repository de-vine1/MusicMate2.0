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
    public class UserPreferencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserPreferencesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPreferences()
        {
            var userId = User
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var preferences = await _context
                .UserPreferences.Where(p => p.UserId == Guid.Parse(userId))
                .ToListAsync();
            return Ok(preferences);
        }

        [HttpPost]
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
