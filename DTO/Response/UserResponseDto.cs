using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Response
{
    public class UserResponseDTO: DateTimes
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
