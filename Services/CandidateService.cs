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
    public class CandidateService
    {
        private DataContext _dataContext;
        private JwtService _jservice;
        private IMemoryCache _cache;
        private IHttpContextAccessor _httpContextAccessor;
        private EmailService _emailService;
        private IConfiguration _iconfiguration;
        public CandidateService(DataContext dataContext, JwtService jservice, IHttpContextAccessor httpContextAccessor, EmailService emailService, IMemoryCache cache, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _jservice = jservice;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _cache = cache;
            _iconfiguration = configuration;

        }

        public async Task<List<Candidate>> GetCandidatesPaged(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Candidate
                .Skip((pageNumber - 1) * 10)
                .Take(10)
                .ToListAsync();
        }

        public async Task<ApiResponse<string>> Register(CandidateDTO candidateDTO)
        {
            var existingCandidates = await _dataContext.Candidate.FirstOrDefaultAsync(u => u.Email == candidateDTO.Email);
            if (existingCandidates != null)
            {
                return ApiResponse<string>.Failure(400, "Email already in candidate.");
            }
            var CandidateName = await _dataContext.Candidate.FirstOrDefaultAsync(u => u.CandidateName == candidateDTO.CandidateName);
            if (CandidateName != null) return ApiResponse<string>.Failure(400, " cndidate name already in candidate");

            var newCandidateDTO = new Candidate
            {
                CandidateName = candidateDTO.CandidateName,
                Email = candidateDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(candidateDTO.Password),
                Create_at = DateTime.UtcNow
            };
            await _dataContext.Candidate.AddAsync(newCandidateDTO);
            await _dataContext.SaveChangesAsync();

            return ApiResponse<string>.Success(200, null, "Account created successfully and confirmation email sent.");
        }

        public async Task<ApiResponse<CandidateResponseDTO>> UpdateCandidate(CandidateDTO candidateDTO)
        {
            var candidateId = _jservice.GetCandidateIdFromHttpContext();
            var candidate = await _dataContext.Candidate.FindAsync(candidateId);

            if (candidate == null) return ApiResponse<CandidateResponseDTO>.Failure(404, " CandidateId NotFound");

            if (!BCrypt.Net.BCrypt.Verify(candidateDTO.Password, candidate.Password))
                return ApiResponse<CandidateResponseDTO>.Failure(400, "Incorrect password");

            if (!string.IsNullOrEmpty(candidateDTO.NewPassword))
            {
                if (candidateDTO.NewPassword.Length < 6)
                    return ApiResponse<CandidateResponseDTO>.Failure(400, "Password must be at least 6 characters long");

                candidate.Password = BCrypt.Net.BCrypt.HashPassword(candidateDTO.NewPassword);
            }

            candidate.CandidateName = candidateDTO.CandidateName;
            candidate.Email = candidateDTO.Email;
            candidate.Create_at = candidate.Create_at;
            candidate.Update_at = DateTime.Now;

            var candidateResponseDTO = new CandidateResponseDTO
            {
                CandidateName = candidate.CandidateName,
                Email = candidate.Email,
                Create_at = candidate.Create_at,
                Update_at = candidate.Update_at,
            };

            await _dataContext.SaveChangesAsync();
            return ApiResponse<CandidateResponseDTO>.Success(200, candidateResponseDTO, "Updata candidate success");
        }

        public async Task<ApiResponse<string>> Login(CandidateLoginDTO candidateLoginDTO)
        {
            var candidate = await _dataContext.Candidate
                .FirstOrDefaultAsync(u => u.CandidateName == candidateLoginDTO.CandidateName);

            if (candidate == null)
                return ApiResponse<string>.Failure(404, "Candidate not found");

            if (!BCrypt.Net.BCrypt.Verify(candidateLoginDTO.Password, candidate.Password))
            {
                return ApiResponse<string>.Failure(400, "Incorrect password");
            }


            var token = _jservice.GenerateJwtToken(candidate);
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

        public async Task<ApiResponse<CandidateResponseDTO>> GetCandidateById()
        {
            var candidateId = _jservice.GetCandidateIdFromHttpContext();
            if (candidateId == null) return ApiResponse<CandidateResponseDTO>.Failure(404, "CandidateId not found");

            var candidate = await _dataContext.Candidate.FirstOrDefaultAsync(u => u.CandidateId == candidateId);
            if (candidate == null) return ApiResponse<CandidateResponseDTO>.Failure(404, "Candidate null");

            var candidateResponseDTO = new CandidateResponseDTO
            {
                CandidateName = candidate.CandidateName,
                Email = candidate.Email,
                Create_at = candidate.Create_at,
                Update_at = candidate.Update_at,
            };

            return ApiResponse<CandidateResponseDTO>.Success(200, candidateResponseDTO, "get Candidate sd success");
        }

    }
}