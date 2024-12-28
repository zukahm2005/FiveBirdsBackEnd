namespace five_birds_be.Models
{
    public partial class Result : DateTimes
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public int QuestionId { get; set; }
        public Question Questions { get; set; }
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
        public int ExamAnswer { get; set; }
        public bool Is_correct { get; set; }
    }
}