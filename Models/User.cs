using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace five_birds_be.Models
{
    public enum Role
    {
        ROLE_CANDIDATE,
        ROLE_ADMIN,
    }

    public class User : DateTimes
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; } = Role.ROLE_CANDIDATE;

        public List<Result> Results { get; set; } = new List<Result>();

        public List<CandidateTest> CandidateTests { get; set; } = new List<CandidateTest>();

    }
}
