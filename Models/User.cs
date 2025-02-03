using System.ComponentModel.DataAnnotations;

namespace MusicMateAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PasswordHash { get; set; } = string.Empty; //Hashed password for better security

        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>(); // one-to-many relationship
    }
}
