namespace five_birds_be.Models{

    public enum Status{
        PAST,
        FAILD,
        PENDING
    }
    public class User_Eaxam : DateTimes{ 
        public int Id { get; set;}
        public int UserId { get; set;}
        public User User{ get; set;}
        public int ExamId { get; set;}
        public Exam Exam{ get; set;}
        public Status TestStatus { get; set;} = Status.PENDING;
        public string ExamTime { get; set;}
        public string ExamDate { get; set;}
    }
}