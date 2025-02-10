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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPreferences(Guid userId)
        {
            var preferences = _context.UserPreferences.Where(up => up.UserId == userId).ToList();
            return Ok(preferences);
        }

        [HttpPost]
        public async Task<IActionResult> SetUserPreference(UserPreferences preference)
        {
            var existingPreference = _context.UserPreferences.FirstOrDefault(up =>
                up.UserId == preference.UserId && up.PreferenceKey == preference.PreferenceKey
            );

            if (existingPreference != null)
            {
                existingPreference.PreferenceValue = preference.PreferenceValue;
                _context.UserPreferences.Update(existingPreference);
            }
            else
            {
                _context.UserPreferences.Add(preference);
            }

            await _context.SaveChangesAsync();
            return Ok(preference);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserPreference(Guid id, UserPreferences preference)
        {
            if (id != preference.Id)
            {
                return BadRequest();
            }

            var existingPreference = await _context.UserPreferences.FindAsync(id);
            if (existingPreference == null)
            {
                return NotFound();
            }

            existingPreference.PreferenceValue = preference.PreferenceValue;
            _context.UserPreferences.Update(existingPreference);
            await _context.SaveChangesAsync();

            return Ok(existingPreference);
        }
    }
}
