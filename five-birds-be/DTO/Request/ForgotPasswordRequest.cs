using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Request
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "The email field is required.")]
        [EmailAddress(ErrorMessage = "The email field is not a valid email address.")]
        public string Email { get; set; }
    }
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
     public class VerifyEmail
{
    public string Email { get; set; } // Email của người dùng để liên kết với OTP
    public string Otp { get; set; }   // OTP cần xác thực
}

}
