namespace five_birds_be.Dto{
    public class AnswerDTO{
         public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string CorrectAnswer { get; set; }
    }
}