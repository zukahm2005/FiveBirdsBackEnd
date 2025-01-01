using five_birds_be.Models;

namespace five_birds_be.DTO.Response
{
    public class UserExamResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public Status TestStatus { get; set; }
        public string ExamTime { get; set; }
        public string ExamDate { get; set; }
    }
}