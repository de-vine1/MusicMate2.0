using System.ComponentModel.DataAnnotations;

namespace MusicMateAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Generate a new Guid when a user is created

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PasswordHash { get; set; } = string.Empty; // Hashed password for better security

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordSalt { get; set; } // Field to store password salt

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>(); // One-to-many relationship
    }
}
