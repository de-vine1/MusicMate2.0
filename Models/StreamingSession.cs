using System.ComponentModel.DataAnnotations.Schema;

namespace MusicMateAPI.Models
{
    public class StreamingSession
    {
        public int id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey("Song")]
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    }
}
