using five_birds_be.Models;

namespace five_birds_be.DTO.Response
{
    public class ExamResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public List<QuestionResponse> Question { get; set; } = new List<QuestionResponse>();
    }
}