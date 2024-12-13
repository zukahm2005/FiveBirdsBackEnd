using System.ComponentModel.DataAnnotations;

namespace five_birds_be.Dto
{
    public class CandidateDTO
    {
        [Required(ErrorMessage = "CandidateName is required")]
        public string CandidateName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public DateTime Update_at { get; set; }
    }
}
