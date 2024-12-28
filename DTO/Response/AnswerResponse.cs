namespace five_birds_be.DTO.Response
{
    public class AnswerResponse
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string QuestionExam { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswer { get; set; }
    }
}