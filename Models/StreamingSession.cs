using System.ComponentModel.DataAnnotations.Schema;

namespace MusicMateAPI.Models
{
    public class StreamingSession
    {
        public int Id { get; set; }  

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey("Song")]
        public int SongId { get; set; }  // This should be int to match Song.Id type
        public Song Song { get; set; } = null!;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    }
}
