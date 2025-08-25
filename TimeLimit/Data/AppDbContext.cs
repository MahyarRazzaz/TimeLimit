using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TimeLimit.Models;

namespace TimeLimit.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Request> Requests => Set<Request>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // یکتا بودن شماره تلفن در جدول User
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.PhoneNumber)
                        .IsUnique();

            // یک به چند بین User و OtpRequest
            modelBuilder.Entity<Request>()
                        .HasOne(r => r.User)          // هر OtpRequest به یک User
                        .WithMany(u => u.Requests) // هر User می‌تواند چند OtpRequest داشته باشد
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            // می‌تونیم ستون TargetPhoneNumber هم محدودیت داشته باشه
            modelBuilder.Entity<Request>()
                        .Property(r => r.TargetPhoneNumber)
                        .HasMaxLength(20)
                        .IsRequired();
        }
    }
}