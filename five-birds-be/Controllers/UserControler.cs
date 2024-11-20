using System.IdentityModel.Tokens.Jwt;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
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

            if (users == null || !users.Any()) return NotFound(ApiResponse<List<User>>.Failure(404, "No users found"));

            return Ok(ApiResponse<List<User>>.Success(200, users, "Users retrieved successfully"));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            var postUser = await _userService.Register(user);

            if (postUser.ErrorCode == 400) return BadRequest(postUser);

            return Ok(postUser);
        }

        [HttpPut("update")]
        [Authorize(Roles = "ROLE_USER")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO user)
        {
            var data = await _userService.UpdateUser(user);

            if (data.ErrorCode == 404) return NotFound(data);

            if (data.ErrorCode == 400) return BadRequest(data);

            return Ok(data);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var data = await _userService.Login(userDTO);

            if (data.ErrorCode == 400) return BadRequest(data);

            return Ok(data);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("token");
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_USER")]
        public async Task<IActionResult> GetUserById()
        {
            var data = await _userService.GetUserById();

            return Ok(data);
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var data = await _userService.ForgotPassword(request.Email);

            if (data.ErrorCode == 404) return NotFound(data);

            return Ok(data);
        }
        [HttpGet("checktoken")]
        public async Task<IActionResult> CheckToken()
        {
            try
            {
                string? token = Request.Cookies["token"];

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token không tồn tại trong cookie." });
                }

                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ReadJwtToken(token).Claims;

                var resultList = claims.Select(c => new
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList();

                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi kiểm tra token", error = ex.Message });
            }
        }

        

    }
}