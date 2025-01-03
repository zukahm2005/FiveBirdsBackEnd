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
        Task<ApiResponse<string>> SendEmailCandidate(int id, EmailRequest body);

    }

    public class CandidateService : ICandidateService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly EmailService _emailService;

        private readonly IConfiguration _configuration;

        public CandidateService(DataContext context, IWebHostEnvironment env, EmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<string>> CreateCandidateAsync(CandidateRequest request)
        {
            string cvUrl = null;

            if (request.CvFile != null)
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(request.CvFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return ApiResponse<string>.Failure(400, "File CV phải là PDF hoặc hình ảnh.");
                }

                using (var fileStream = request.CvFile.OpenReadStream())
                {
                    var cloudinaryService = new CloudinaryService(_configuration);
                    cvUrl = await cloudinaryService.UploadImageAsync(fileStream, request.CvFile.FileName);
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

            var user = new User
            {
                UserName = username,
                Password = randomPassword,
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
                Birthday = request.Birthday,
                Education = request.Education,
                Experience = request.Experience,
                ApplyLocation = request.ApplyLocation,
                CvFilePath = cvUrl,
                UserId = user.UserId
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            string adminEmail = "maituanvu141@gmail.com";
            string emailSubject = "Notifity: Candidate up CV";
            string emailBody = $@"
        <h2>Information candidate</h2>
        <p><strong>Full Name:</strong> {request.FullName}</p>
        <p><strong>Email:</strong> {request.Email}</p>
        <p><strong>Username:</strong> {username}</p>
        <p><strong>Password:</strong> {randomPassword}</p>
        <p>File CV: <a href='{cvUrl}'>Download CV</a></p>
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
                    Birthday = c.Birthday,
                    Education = c.Education,
                    Experience = c.Experience,
                    ApplyLocation = c.ApplyLocation,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<CandidateResponse>>.Success(200, candidates);
        }

        public async Task<ApiResponse<CandidateResponse>> GetCandidateByIdAsync(int id)
        {
            var candidate = await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return ApiResponse<CandidateResponse>.Failure(404, "Candidate not found.");
            }

            var response = new CandidateResponse
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Phone = candidate.Phone,
                Birthday = candidate.Birthday,
                Education = candidate.Education,
                Experience = candidate.Experience,
                ApplyLocation = candidate.ApplyLocation,
                 CvFilePath = candidate.CvFilePath, 
                CreatedAt = candidate.CreatedAt,
                User = new UserResponseDTO
                {
                    UserId = candidate.User.UserId,
                    UserName = candidate.User.UserName,
                    Email = candidate.User.Email,
                    Password = candidate.User.Password
                }
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
            candidate.Birthday = request.Birthday;
            candidate.Education = request.Education;
            candidate.Experience = request.Experience;
            candidate.ApplyLocation = request.ApplyLocation;

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

        public async Task<ApiResponse<string>> SendEmailCandidate(int id, EmailRequest emailRequest)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(cd => cd.Id == id);
            if (candidate == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy ID ứng viên");

            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == candidate.UserId);
            if (user == null) return ApiResponse<string>.Failure(404, "không tìm thấy Id người dùng");

            var body = new EmailResponse
            {
                examTitle = emailRequest.examTitle,
                comment = emailRequest.comment,
                selectedTime = emailRequest.selectedTime,
                selectedDate = emailRequest.selectedDate,
                UserName = user.UserName,
                Password = user.Password
            };

            var email = candidate.Email;
            if (string.IsNullOrEmpty(email))
                return ApiResponse<string>.Failure(404, "Email của ứng viên không tồn tại");

            await _emailService.SendEmailAsyncObject(email, "Exam schedule announcement", body);

            return ApiResponse<string>.Success(200, "Gửi email thành công");
        }
    }
}
