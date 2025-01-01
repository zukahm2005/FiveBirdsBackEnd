using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace five_birds_be.Models{
    public class Question: DateTimes{
        public int Id { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public string QuestionExam { get; set; }
        public int Point { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}