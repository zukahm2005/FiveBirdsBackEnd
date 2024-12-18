namespace five_birds_be.Models
{
    public partial class Result: DateTimes
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User user { get; set; }
        public int ExamId { get; set; }
        public Exam exam { get; set; }
        public int QuestionId { get; set; }
        public Question Auestions { get; set; }
        public int Answers { get; set; }
        public Answer Answer { get; set; }
        public string ExamAnswer{ get; set; }
        public bool Is_correct{ get; set; }
    }
}