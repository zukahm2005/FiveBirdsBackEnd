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
        public string ApplyLocation { get; set; }
        public UserResponseDTO User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}