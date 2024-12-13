using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Response
{
    public class CandidateResponseDTO
    {
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public DateTime Create_at { get; set; }

        public DateTime Update_at { get; set;}
    }
}
