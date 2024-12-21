using five_birds_be.Data;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public interface ICandidateService
    {
        Task<ApiResponse<string>> CreateCandidateAsync(CandidateRequest request);
        Task<ApiResponse<List<CandidateResponse>>> GetCandidatesAsync();
        Task<ApiResponse<CandidateResponse>> GetCandidateByIdAsync(int id);
        Task<ApiResponse<string>> UpdateCandidateAsync(int id, CandidateRequest request);
        Task<ApiResponse<string>> DeleteCandidateAsync(int id);
    }

    public class CandidateService : ICandidateService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        public CandidateService(DataContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<ApiResponse<string>> CreateCandidateAsync(CandidateRequest request)
        {
            string filePath = null;

            if (request.CvFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(request.CvFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return ApiResponse<string>.Failure(400, "File CV phải là PDF hoặc hình ảnh.");
                }

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.CvFile.CopyToAsync(fileStream);
                }
            }

            var emailPrefix = request.Email.Split('@')[0];
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.UserName == emailPrefix);
            string username = emailPrefix;

            if (existingUser != null)
            {
                username = $"{emailPrefix}_{new Random().Next(1000, 9999)}";
            }

            var randomPassword = GenerateRandomPassword();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            var user = new User
            {
                UserName = username,
                Password = hashedPassword,
                Email = request.Email,
                Role = Role.ROLE_CANDIDATE
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var candidate = new Candidate
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Education = request.Education,
                Experience = request.Experience,
                CvFilePath = filePath,
                UserId = user.UserId
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            string adminEmail = "maituanvu141@gmail.com";
            string emailSubject = "Thông báo: Có người nộp CV mới";
            string emailBody = $@"
        <h2>Thông tin ứng viên mới</h2>
        <p><strong>Họ tên:</strong> {request.FullName}</p>
        <p><strong>Email:</strong> {request.Email}</p>
        <p><strong>Username:</strong> {username}</p>
        <p><strong>Password:</strong> {randomPassword}</p>
        <p>File CV đã được tải lên: <a href='{filePath}'>Tải xuống CV</a></p>
    ";

            var emailService = new EmailService();
            await emailService.SendEmailAsync(adminEmail, emailSubject, emailBody);

            return ApiResponse<string>.Success(200, "Send success");
        }

        private string GenerateRandomPassword()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }


        public async Task<ApiResponse<List<CandidateResponse>>> GetCandidatesAsync()
        {
            var candidates = await _context.Candidates
                .Select(c => new CandidateResponse
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Education = c.Education,
                    Experience = c.Experience,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<CandidateResponse>>.Success(200, candidates);
        }

        public async Task<ApiResponse<CandidateResponse>> GetCandidateByIdAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return ApiResponse<CandidateResponse>.Failure(404, "Candidate not found.");

            var response = new CandidateResponse
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Phone = candidate.Phone,
                Education = candidate.Education,
                Experience = candidate.Experience,
                CreatedAt = candidate.CreatedAt
            };

            return ApiResponse<CandidateResponse>.Success(200, response);
        }

        public async Task<ApiResponse<string>> UpdateCandidateAsync(int id, CandidateRequest request)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy ứng viên.");

            candidate.FullName = request.FullName;
            candidate.Email = request.Email;
            candidate.Phone = request.Phone;
            candidate.Education = request.Education;
            candidate.Experience = request.Experience;

            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "Hồ sơ ứng viên đã được cập nhật.");
        }

        public async Task<ApiResponse<string>> DeleteCandidateAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy ứng viên.");

            _context.Candidates.Remove(candidate);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "Ứng viên đã được xóa.");
        }
    }
}
