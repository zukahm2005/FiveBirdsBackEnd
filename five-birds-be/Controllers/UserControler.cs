using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all/{pageNumber}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAllUser(int pageNumber)
        {
            var users = await _userService.GetUsersPaged(pageNumber);
            if (users == null || !users.Any())
            {
                return NotFound(ApiResponse<List<User>>.Failure(404, "No users found"));
            }
            return Ok(ApiResponse<List<User>>.Success(200, users, "Users retrieved successfully"));
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            var postUser = await _userService.Register(user);

            // Kiểm tra mã lỗi trong ApiResponse
            if (postUser.ErrorCode == 400)
            {
                return BadRequest(postUser); 
            }

            return Ok(postUser); 
        }



        [HttpPut("updata")]
        [Authorize(Roles ="ROLE_USER")]

        public async Task<IActionResult> UpdataUser([FromBody] UserDTO user)
        {
            var data = await _userService.UpdataUser(user);
            if (data.ErrorCode == 404)
            {
                return NotFound(data);
            }
            return Ok(data);
        }
        [HttpDelete("delete")]
        [Authorize(Roles ="ROLE_ADMIN")]
        public async Task<IActionResult> DeleteUser()
        {
            var data = await _userService.DeleteUser();
            if (data.ErrorCode == 404)
            {
                return NotFound(data);
            }
            return NoContent();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var data = await _userService.Login(userDTO);

            if (data.ErrorCode == 400)
            {
                return BadRequest(data);
            }
            return Ok(data);
        }
        [HttpGet]
        [Authorize(Roles ="ROLE_ADMIN, ROLE_USER")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var data = await _userService.GetUserById(id);
            return Ok(data);
        }
    }
}