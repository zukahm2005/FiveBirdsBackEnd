namespace five_birds_be.Models
{
    public class OtpRecord
    {
        public int Id { get; set; }  // Khóa chính (Primary Key)
        public int UserId { get; set; }  // ID người dùng liên kết với mã OTP
        public string Otp { get; set; }  // Mã OTP
        public DateTime ExpirationTime { get; set; }  // Thời gian hết hạn của mã OTP

        public User User { get; set; }  = new User();
    }
}