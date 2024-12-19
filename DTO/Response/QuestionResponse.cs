using five_birds_be.Models;

namespace five_birds_be.DTO.Response
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string TitleExam { get; set; }
        public string QuestionExam { get; set; }
        public string Point { get; set; }
        public List<AnswerResponse> Answers { get; set; } = new List<AnswerResponse>();
    }
}