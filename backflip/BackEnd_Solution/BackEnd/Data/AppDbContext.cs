using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<PhotoShare> PhotoShares => Set<PhotoShare>();
        public DbSet<Token> Tokens => Set<Token>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Photos)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhotoShare>()
                .HasOne(ps => ps.Photo)
                .WithMany(p => p.Shares)
                .HasForeignKey(ps => ps.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhotoShare>()
                .HasOne(ps => ps.SharedWithUser)
                .WithMany(u => u.PhotosSharedWithMe)
                .HasForeignKey(ps => ps.SharedWithUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PhotoShare>()
                .HasIndex(ps => new { ps.PhotoId, ps.SharedWithUserId })
                .IsUnique();

            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithOne(u => u.Token)
                .HasForeignKey<Token>(t => t.Email)
                .HasPrincipalKey<User>(u=>u.Email)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
