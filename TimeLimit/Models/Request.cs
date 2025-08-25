using System.ComponentModel.DataAnnotations;

namespace TimeLimit.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string TargetPhoneNumber { get; set; } = string.Empty;

        public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; } = null!;
    }
}
