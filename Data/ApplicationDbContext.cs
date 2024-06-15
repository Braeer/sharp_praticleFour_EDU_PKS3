using Microsoft.EntityFrameworkCore;
using Practice_web.Models;

namespace Practice_web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .ToTable("user");

            modelBuilder.Entity<Message>()
                .ToTable("message")
                .HasOne(m => m.FromUser)
                .WithMany(fu => fu.SentMessages)
                .HasForeignKey(m => m.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.ToUser)
                .WithMany(fu => fu.ReceiveMessages)
                .HasForeignKey(m => m.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
