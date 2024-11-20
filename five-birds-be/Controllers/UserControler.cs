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
        [Authorize(Roles ="ROLE_ADMIN")]
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

            if (postUser == null)
            {
                return BadRequest(ApiResponse<UserResponseDTO>.Failure(400, "Failed to create user"));
            }
            return Ok(postUser);
        }

        [HttpPut("update/{id}")]
        [Authorize("ROLE_USER")]

        public async Task<IActionResult> UpdataUser([FromRoute] int id, [FromBody] UserDTO user)
        {
            var data = await _userService.UpdataUser(id, user);
            if (data == null)
            {
                return BadRequest(ApiResponse<User>.Failure(400, "Failed to put user"));
            }
            return Ok(ApiResponse<User>.Success(201, data, "Users updata successfully"));
        }
        [HttpDelete("delete/{id}")]
        [Authorize("ROLE_ADMIN")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var data = await _userService.DeleteUser(id);
            if (data == null)
            {
                return BadRequest(ApiResponse<User>.Failure(400, "Failed to delete user"));
            }
            return Ok(ApiResponse<User>.Success(200, null, "Users delete successfully "));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var data = await _userService.Login(userDTO);

            if (data == null)
            {
                // Return Unauthorized for failed login attempt
                return Unauthorized(ApiResponse<User>.Failure(401, "Invalid credentials"));
            }
            return Ok(data);
        }
        [HttpGet]
        [Authorize("ROLE_ADMIN, ROLE_USER")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var data = await _userService.GetUserById(id);
            return Ok(data);
        }
    }
}