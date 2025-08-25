using System.ComponentModel.DataAnnotations;

namespace TimeLimit.Dtos
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "شماره تلفن اجباری است")]
        [MaxLength(20, ErrorMessage = "شماره تلفن نمی‌تواند بیشتر از 20 کاراکتر باشد")]
        public string TargetPhoneNumber { get; set; } = string.Empty;
    }
}