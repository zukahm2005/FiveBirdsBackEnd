namespace five_birds_be.DTO.Response
{
    public class CandidateTestRespone
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int Point { get; set; }
        public bool IsPast { get; set; }
    }
}