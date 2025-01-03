using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Response
{
    public class UserResponseDTO: DateTimes
    {
         public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
