using Microsoft.EntityFrameworkCore;
using MusicMateAPI.Models;

namespace MusicMateAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StreamingSession> StreamingSessions { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many Relationship between Playlist and Songs
            modelBuilder.Entity<Playlist>().HasMany(p => p.Songs).WithMany(s => s.Playlists);

            // One-to-Many Relationship between User and Playlists (using Guid)
            modelBuilder
                .Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId) // Foreign key should be of type Guid
                .IsRequired();

            // One-to-Many Relationship between StreamingSession and User (using Guid)
            modelBuilder
                .Entity<StreamingSession>()
                .HasOne(ss => ss.User)
                .WithMany()
                .HasForeignKey(ss => ss.UserId); // Foreign key is Guid

            // One-to-Many Relationship between StreamingSession and Song (using int for SongId)
            modelBuilder
                .Entity<StreamingSession>()
                .HasOne(ss => ss.Song)
                .WithMany()
                .HasForeignKey(ss => ss.SongId); // Foreign key is int
        }
    }
}
