using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Response
{
   public partial class ResultResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int ExamAnswer{ get; set; }
        public bool Is_correct{ get; set; }
    }
}