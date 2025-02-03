using System.ComponentModel.DataAnnotations;

namespace MusicMateAPI.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Artist { get; set; } = string.Empty;

        [Required]
        public string? Album { get; set; }

        [Required]
        public string Genre { get; set; } = string.Empty;

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public string ReleaseDate { get; set; } = string.Empty;

        [Required]
        public string CoverArt { get; set; } = string.Empty;

        [Required]
        public string AudioFile { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public string SourceUrl { get; set; } = string.Empty;

        public ICollection<Playlist>? Playlists { get; set; } = new List<Playlist>(); // many-to-many relationship
    }
}
