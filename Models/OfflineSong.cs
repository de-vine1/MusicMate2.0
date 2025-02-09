using System;

namespace MusicMateAPI.Models
{
    public class OfflineSong
    {
        public Guid Id { get; set; }
        public string SongId { get; set; } = string.Empty; // Ensure non-nullable
        public string SongUrl { get; set; } = string.Empty; // Ensure non-nullable
        public DateTime CachedAt { get; set; }
    }
}
