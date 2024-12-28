namespace five_birds_be.Models
{
    public class Answer: DateTimes
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question{ get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswer { get; set; }
    }
}