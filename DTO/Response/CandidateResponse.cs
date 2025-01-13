using five_birds_be.DTO.Request;
using five_birds_be.Models;

namespace five_birds_be.DTO.Response
{
    public class CandidateResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Birthday { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }

        public CandidatePositionResponse CandidatePosition { get; set; }

        public string CvFilePath { get; set; }
        public StatusEmail StatusEmail { get; set; }

        public UserResponseDTO User { get; set; }

        public bool? IsPast { get; set; }

        public bool IsInterview { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}