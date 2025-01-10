namespace five_birds_be.DTO
{
    public class QuestionImportDTO
    {
        public int ExamId { get; set; }
        public string QuestionText { get; set; }
        public int Point { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public int CorrectAnswer { get; set; }
    }
}
