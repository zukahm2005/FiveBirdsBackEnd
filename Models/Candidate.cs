namespace five_birds_be.Models
{
    public enum StatusEmail
    {
        PENDING,
        SUCCESS
    }
    public class Candidate
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Birthday { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public string CvFilePath { get; set; }
        public StatusEmail StatusEmail { get; set; } = StatusEmail.PENDING;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User User { get; set; }

        public int CandidatePositionId { get; set; }
        public CandidatePosition CandidatePosition { get; set; }
    }
}