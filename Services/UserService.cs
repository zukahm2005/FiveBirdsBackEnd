using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Jwt;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace five_birds_be.Services
{
    public class UserService
    {
        private DataContext _dataContext;
        private JwtService _jservice;
        private IMemoryCache _cache;
        private IHttpContextAccessor _httpContextAccessor;
        private EmailService _emailService;
        private IConfiguration _iconfiguration;
        public UserService(DataContext dataContext, JwtService jservice, IHttpContextAccessor httpContextAccessor, EmailService emailService, IMemoryCache cache, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _jservice = jservice;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _cache = cache;
            _iconfiguration = configuration;

        }

        public async Task<List<User>> GetUserPaged(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.User
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ApiResponse<string>> Register(UserDTO userDTO)
        {
            var existingUsers = await _dataContext.User.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (existingUsers != null)
            {
                return ApiResponse<string>.Failure(400, "Email already in User.");
            }
            var UserName = await _dataContext.User.FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (UserName != null) return ApiResponse<string>.Failure(400, " UserName already in User");

            var newUserDTO = new User
            {
                UserName =userDTO.UserName,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Create_at = DateTime.UtcNow
            };
            await _dataContext.User.AddAsync(newUserDTO);
            await _dataContext.SaveChangesAsync();

            return ApiResponse<string>.Success(200, null, "Account created successfully and confirmation email sent.");
        }

        public async Task<ApiResponse<UserResponseDTO>> UpdateUser(UserDTO userDTO)
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            var user = await _dataContext.User.FindAsync(userId);

            if (user == null) return ApiResponse<UserResponseDTO>.Failure(404, " UserId NotFound");

            if (!BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password))
                return ApiResponse<UserResponseDTO>.Failure(400, "Incorrect password");

            if (!string.IsNullOrEmpty(userDTO.NewPassword))
            {
                if (userDTO.NewPassword.Length < 6)
                    return ApiResponse<UserResponseDTO>.Failure(400, "Password must be at least 6 characters long");

                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.NewPassword);
            }

            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.Create_at = user.Create_at;
            user.Update_at = DateTime.Now;

            var userResponseDTO = new UserResponseDTO
            {
                UserName= user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            await _dataContext.SaveChangesAsync();
            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "Updata user success");
        }

        public async Task<ApiResponse<string>> Login(UserLoginDTO userLoginDTO)
        {
            var user = await _dataContext.User
                .FirstOrDefaultAsync(u => u.UserName == userLoginDTO.UserName);

            if (user == null)
                return ApiResponse<string>.Failure(404, "user not found");

            if (!BCrypt.Net.BCrypt.Verify(userLoginDTO.Password, user.Password))
            {
                return ApiResponse<string>.Failure(400, "Incorrect password");
            }


            var token = _jservice.GenerateJwtToken(user);
            SetAuthCookie(token);

            return ApiResponse<string>.Success(200, token, "Đăng nhập thành công.");
        }


        private void SetAuthCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("token", token, cookieOptions);
        }

        public async Task<ApiResponse<UserResponseDTO>> GetUserById()
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            if (userId == null) return ApiResponse<UserResponseDTO>.Failure(404, "UserId not found");

            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return ApiResponse<UserResponseDTO>.Failure(404, "User null");

            var userResponseDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "get UserId success");
        }

    }
}