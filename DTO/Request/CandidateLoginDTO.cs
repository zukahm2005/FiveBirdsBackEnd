using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Request
{
    public class CandidateLoginDTO
    {
        [Required(ErrorMessage = "CandidateName is required")]
        public string CandidateName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
