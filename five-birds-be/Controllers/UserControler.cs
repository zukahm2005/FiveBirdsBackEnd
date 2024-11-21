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
        public IActionResult Logout()
        {
            try
            {
                // Xóa cookie bằng cách set thời gian hết hạn trong quá khứ
                Response.Cookies.Append("token", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Chỉ hoạt động trên HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1) // Set thời gian hết hạn trong quá khứ
                });

                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi logout", error = ex.Message });
            }
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
            var data = await _userService.ForgotPassword(request);

            if (data.ErrorCode == 404) return NotFound(data);

            return Ok(data);
        }

        [HttpPost("checkotp")]
        public async Task<IActionResult> CheckOtp([FromBody] VerifyOtpRequest request){
            var data = await _userService.VerifyOtpAndResetPassword(request);
            if(data.ErrorCode == 404) return NotFound(data);
            if(data.ErrorCode == 400) return BadRequest(data);
            return Ok(data);
        }

       [HttpGet("checktoken")]
        public IActionResult CheckToken()
        {
            try
            {
                // Lấy token từ cookie
                string? token = Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return Ok(new { isLoggedIn = false, message = "Chưa đăng nhập." });
                }

                // Giải mã token để kiểm tra
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var claims = handler.ReadJwtToken(token).Claims;

                    var resultList = claims.Select(c => new
                    {
                        Type = c.Type,
                        Value = c.Value
                    }).ToList();

                    return Ok(new { isLoggedIn = true, claims = resultList, message = "Đã đăng nhập." });
                }
                catch
                {
                    return BadRequest(new { isLoggedIn = false, message = "Token không hợp lệ." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isLoggedIn = false, message = "Lỗi khi kiểm tra token.", error = ex.Message });
            }
        }

    }
}