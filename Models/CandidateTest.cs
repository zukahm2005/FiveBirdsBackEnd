namespace five_birds_be.Models
{
    public class CandidateTest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public List<Result> Results { get; set; } = new List<Result>();
        public int Point { get; set; }
        public bool IsPast { get; set; }
    }
}
