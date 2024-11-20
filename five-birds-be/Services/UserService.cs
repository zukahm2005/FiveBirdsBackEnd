using System.IdentityModel.Tokens.Jwt;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Jwt;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace five_birds_be.Services
{


    public class UserService
    {
        private DataContext _dataContext;
        private JwtService _jservice;
        private IHttpContextAccessor _httpContextAccessor;
        public UserService(DataContext dataContext, JwtService jservice, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _jservice = jservice;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetUsersPaged(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Users
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
        }


        public async Task<ApiResponse<UserResponseDTO>> Register(UserDTO userDTO)
        {
            var username = await _dataContext.Users
                .FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (username != null) return ApiResponse<UserResponseDTO>.Failure(400, "UserName already exists");

            var email = await _dataContext.Users
             .FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (email != null) return ApiResponse<UserResponseDTO>.Failure(400, "Email already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            var user = new User
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email,
                Password = hashedPassword,
            };

            var users = await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            var responseUserDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Create_at
            };

            return ApiResponse<UserResponseDTO>.Success(200, responseUserDTO, "User registered successfully");
        }

        public async Task<ApiResponse<UserResponseDTO>> UpdataUser(UserDTO userDTO)
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            var user = await _dataContext.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserResponseDTO>.Failure(404, " UserId NotFound");
            }
            user.UserName = userDTO.UserName;
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            user.Email = userDTO.Email;
            user.Create_at = user.Create_at;
            user.Update_at = DateTime.Now; 

            var userResponseDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                Create_at = user.Create_at,
                Update_at = user.Update_at,
            };

            await _dataContext.SaveChangesAsync();
            return ApiResponse<UserResponseDTO>.Success(200, userResponseDTO, "Updata user success");
        }

        public async Task<ApiResponse<string>> DeleteUser()
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            var user = await _dataContext.Users.FindAsync(userId);

            if (user == null)
            {
                return ApiResponse<string>.Failure(404, " UserId NotFound");
            }
            _dataContext.Users.Remove(user);

            await _dataContext.SaveChangesAsync();

            return ApiResponse<string>.Success(204, "Delete User Succuss");
        }
        public async Task<ApiResponse<string>> Login(UserLoginDTO userDTO)
        {
            var user = await _dataContext.Users
            .FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);

            if (user == null)
            {
                return ApiResponse<string>.Failure(400, "Incorrect username or password");
            }

            var passwordMatch = BCrypt.Net.BCrypt.Verify(userDTO.Password, user.Password);

            if (!passwordMatch)
            {
                return ApiResponse<string>.Failure(400, "Incorrect username or password");
            }

            var token = _jservice.GenerateJwtToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("access_token", token, cookieOptions);

            var responseUserDTO = new UserResponseDTO
            {
                UserName = user.UserName,
                Email = user.Email
            };

            return ApiResponse<string>.Success(200, token, "User logged in successfully.");
        }


        public async Task<ApiResponse<string>> GetUserById(int id)
        {
            var userId = _jservice.GetUserIdFromHttpContext();
            if (userId == null)
            {
                return ApiResponse<string>.Failure(400, "userid not found");
            }
            return ApiResponse<string>.Success(200, userId.ToString(), "get User id success");
        }



        public User ConvertToEntity(UserResponseDTO userResponseDTO)
        {
            return new User
            {
                UserName = userResponseDTO.UserName,
                Email = userResponseDTO.Email,
            };
        }


    }
}