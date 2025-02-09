using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicMateAPI.Data;
using MusicMateAPI.Models;

namespace MusicMateAPI.Services
{
    public interface IOfflineService
    {
        Task<IEnumerable<OfflineSong>> GetOfflineSongsAsync();
        Task CacheSongAsync(string songId);
    }

    public class OfflineService : IOfflineService
    {
        private readonly AppDbContext _context;
        private readonly SpotifyService _spotifyService;

        public OfflineService(AppDbContext context, SpotifyService spotifyService)
        {
            _context = context;
            _spotifyService = spotifyService;
        }

        public async Task<IEnumerable<OfflineSong>> GetOfflineSongsAsync()
        {
            return await _context.OfflineSongs.ToListAsync();
        }

        public async Task CacheSongAsync(string songId)
        {
            var songUrl = await _spotifyService.GetSongUrlAsync(songId);
            if (string.IsNullOrEmpty(songUrl))
            {
                throw new Exception("Song URL not found");
            }

            var offlineSong = new OfflineSong
            {
                Id = Guid.NewGuid(),
                SongId = songId,
                SongUrl = songUrl,
                CachedAt = DateTime.UtcNow,
            };

            _context.OfflineSongs.Add(offlineSong);
            await _context.SaveChangesAsync();
        }
    }
}
