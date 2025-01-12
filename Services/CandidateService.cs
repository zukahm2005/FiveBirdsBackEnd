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
        Task<ApiResponse<List<CandidateResponse>>> GetCandidatesPage(int pageNumber, int pageSize, StatusEmail? statusEmail, int? CandidatePositionId, DateTime? startDate, DateTime? endDate);
        Task<ApiResponse<string>> SendEmailInterviewSchedule(int CandidateId, EmailRequest2 emailRequest);
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

            var candidatePosition = await _context.CandidatePositions
               .FirstOrDefaultAsync(cp => cp.Id == request.CandidatePositionId);

            if (candidatePosition == null)
            {
                return ApiResponse<string>.Failure(404, "Không tìm thấy vị trí ứng tuyển.");
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
                CvFilePath = cvUrl,
                UserId = user.UserId,
                CandidatePosition = candidatePosition
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
                <p>File CV: <a href='{cvUrl}'>CV Here</a></p>
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
               .Include(c => c.CandidatePosition)
               .Include(c => c.User)
               .Select(c => new CandidateResponse
               {
                   Id = c.Id,
                   FullName = c.FullName,
                   Email = c.Email,
                   Phone = c.Phone,
                   Birthday = c.Birthday,
                   Education = c.Education,
                   Experience = c.Experience,
                   CvFilePath = c.CvFilePath,
                   StatusEmail = c.StatusEmail,
                   CreatedAt = c.CreatedAt,
                   CandidatePosition = c.CandidatePosition != null ? new CandidatePositionResponse
                   {
                       Id = c.CandidatePosition.Id,
                       Name = c.CandidatePosition.Name
                   } : null,
                   User = c.User != null ? new UserResponseDTO
                   {
                       UserId = c.User.UserId,
                       UserName = c.User.UserName,
                       Password = c.User.Password,
                       Email = c.User.Email
                   } : null
               })
       .ToListAsync();

            return ApiResponse<List<CandidateResponse>>.Success(200, candidates);
        }

        public async Task<ApiResponse<List<CandidateResponse>>> GetCandidatesPage(int pageNumber, int pageSize, StatusEmail? statusEmail, int? CandidatePositionId, DateTime? startDate, DateTime? endDate)
        {
            Console.WriteLine("sdasdfasdf" + statusEmail);

            var query = _context.Candidates
                .Include(c => c.CandidatePosition)
                .Include(c => c.User)
                .AsQueryable();

            if (statusEmail == StatusEmail.SUCCESS || statusEmail == StatusEmail.PENDING)
            {
                query = query.Where(c => c.StatusEmail == statusEmail);
            }

            if (CandidatePositionId.HasValue)
            {
                query = query.Where(c => c.CandidatePosition.Id == CandidatePositionId.Value);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= startDate.Value && c.CreatedAt <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= endDate.Value);
            }


            var candidates = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CandidateResponse
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Birthday = c.Birthday,
                    Education = c.Education,
                    Experience = c.Experience,
                    CvFilePath = c.CvFilePath,
                    StatusEmail = c.StatusEmail,
                    CreatedAt = c.CreatedAt,
                    CandidatePosition = c.CandidatePosition != null ? new CandidatePositionResponse
                    {
                        Id = c.CandidatePosition.Id,
                        Name = c.CandidatePosition.Name
                    } : null,
                    User = c.User != null ? new UserResponseDTO
                    {
                        UserId = c.User.UserId,
                        UserName = c.User.UserName,
                        Password = c.User.Password,
                        Email = c.User.Email
                    } : null
                })
                .ToListAsync();

            return ApiResponse<List<CandidateResponse>>.Success(200, candidates);
        }


        public async Task<ApiResponse<CandidateResponse>> GetCandidateByIdAsync(int id)
        {
            var candidate = await _context.Candidates
               .Include(c => c.User)
               .Include(c => c.CandidatePosition)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
            {
                return ApiResponse<CandidateResponse>.Failure(404, "Candidate not found.");
            }

            var userData = candidate.User;
            if (userData == null)
            {
                return ApiResponse<CandidateResponse>.Failure(404, "User not found for the candidate.");
            }
            var userId = userData.UserId;

            var candidateTest = await _context.CandidateTests.FirstOrDefaultAsync(ct => ct.UserId == userId);
            var IsPast = candidateTest?.IsPast;



            var response = new CandidateResponse
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Phone = candidate.Phone,
                Birthday = candidate.Birthday,
                Education = candidate.Education,
                Experience = candidate.Experience,
                CvFilePath = candidate.CvFilePath,
                CreatedAt = candidate.CreatedAt,
                CandidatePosition = new CandidatePositionResponse
                {
                    Id = candidate.CandidatePosition.Id,
                    Name = candidate.CandidatePosition.Name
                },
                User = new UserResponseDTO
                {
                    UserId = candidate.User.UserId,
                    UserName = candidate.User.UserName,
                    Email = candidate.User.Email,
                    Password = candidate.User.Password
                },
            };

            if (IsPast != null)
            {
                response.IsPast = IsPast;
            }
            return ApiResponse<CandidateResponse>.Success(200, response);
        }

        public async Task<ApiResponse<string>> UpdateCandidateAsync(int id, CandidateRequest request)
        {
            var candidate = await _context.Candidates
                .Include(c => c.CandidatePosition)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy ứng viên.");

            var candidatePosition = await _context.CandidatePositions
                .FirstOrDefaultAsync(cp => cp.Id == request.CandidatePositionId);

            if (candidatePosition == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy vị trí ứng tuyển.");

            candidate.FullName = request.FullName;
            candidate.Email = request.Email;
            candidate.Phone = request.Phone;
            candidate.Birthday = request.Birthday;
            candidate.Education = request.Education;
            candidate.Experience = request.Experience;
            candidate.CandidatePosition = candidatePosition;

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

            candidate.StatusEmail = StatusEmail.SUCCESS;
            await _context.SaveChangesAsync();

            var body = new EmailResponse
            {
                name = candidate.FullName,
                email = candidate.Email,
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

        public async Task<ApiResponse<string>> SendEmailInterviewSchedule(int candidateID, EmailRequest2 emailRequest2)
        {
            var data = await _context.Candidates.FirstOrDefaultAsync(c => c.Id == candidateID);
            if (data == null) return ApiResponse<string>.Failure(404, "candidate id not found");
            string name = data.FullName;
            string email = data.Email;
            string subject = "Interview Schedule Notification";
            await _emailService.SendInterviewSchedule( name ,email, subject, emailRequest2);
            return ApiResponse<string>.Success(200, "send email successfully");
        }

    }
}
