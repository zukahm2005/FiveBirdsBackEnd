namespace five_birds_be.Models
{
    public class CandidatePosition
    {
        public int Id { get; set; }

        public string Name { get; set; }

         public int? CandidateId { get; set; }
        public Candidate Candidate { get; set; }
        public List<Exam> Exams { get; set; } = new List<Exam>();
    }
}