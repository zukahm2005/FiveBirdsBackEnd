namespace five_birds_be.Models
{
    public class Exam : DateTimes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public List<Question> Question { get; set; } = new List<Question>();

    }
}