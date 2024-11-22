namespace five_birds_be.Models
{
    public partial class TrustedDevice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceIdentifier { get; set; }
        public DateTime TrustedUntil { get; set; }
        public DateTime? LastEmailSent { get; set; } 

        public bool IsTrusted { get; set; }
    }
}