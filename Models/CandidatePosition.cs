namespace five_birds_be.Models
{
    public class CandidatePosition
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Candidate> Candidates { get; set; } = new List<Candidate>();

        public List<Exam> Exams { get; set; } = new List<Exam>();
    }
}