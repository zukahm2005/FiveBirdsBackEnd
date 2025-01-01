using five_birds_be.Models;

namespace five_birds_be.DTO.Request{
    public class UserExamRequest{
        public int UserId { get; set;}
        public int ExamId { get; set;}
        public Status TestStatus { get; set;} 
        public string ExamTime { get; set;}
        public string ExamDate { get; set;}
    }
}