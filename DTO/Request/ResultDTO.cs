using System.ComponentModel.DataAnnotations;

namespace five_birds_be.Dto
{
    public partial class ResultDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "ExamId is required")]

        public int ExamId { get; set; }
        [Required(ErrorMessage = "QuestionId is required")]

        public int QuestionId { get; set; }
        [Required(ErrorMessage = "AnswerId is required")]

        public int AnswerId { get; set; }
        
        [Required(ErrorMessage = "ExamAnswer is required")]
        public int ExamAnswer { get; set; }
    }
}