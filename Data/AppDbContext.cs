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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many Relationship between Playlist and Songs
            modelBuilder.Entity<Playlist>().HasMany(p => p.Songs).WithMany(s => s.Playlists);

            // One-to-Many Relationship between User and Playlists
            modelBuilder
                .Entity<User>()
                .HasMany(u => u.Playlists)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            // One-to-Many Relationship between StreamingSession and User
            modelBuilder
                .Entity<StreamingSession>()
                .HasOne(ss => ss.User)
                .WithMany()
                .HasForeignKey(ss => ss.UserId);

            // One-to-Many Relationship between StreamingSession and Song
            modelBuilder
                .Entity<StreamingSession>()
                .HasOne(ss => ss.Song)
                .WithMany()
                .HasForeignKey(ss => ss.SongId);
        }
    }
}
