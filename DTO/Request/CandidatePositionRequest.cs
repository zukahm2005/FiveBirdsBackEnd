using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO
{
    public class CandidatePositionRequest 
    { 
        [Required]
        public string Name { get; set; }
    }
}