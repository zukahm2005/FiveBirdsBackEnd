using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Request
{
    public class CandidateRequest
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Education { get; set; }
        public string Experience { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

}