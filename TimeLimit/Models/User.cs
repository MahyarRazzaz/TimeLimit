using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TimeLimit.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation Property → یک User می‌تواند چند OtpRequest داشته باشد
        public ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}

