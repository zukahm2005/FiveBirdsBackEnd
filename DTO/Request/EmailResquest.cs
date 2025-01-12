namespace five_birds_be.DTO.Request
{
    public class EmailRequest
    {
        public string examTitle { get; set; }
        public string comment { get; set; }
        public string selectedTime { get; set; }
        public string selectedDate { get; set; }
    }
    public class EmailRequest2
    {
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
