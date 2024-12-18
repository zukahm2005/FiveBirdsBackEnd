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

        public CandidateService(DataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> CreateCandidateAsync(CandidateRequest request)
        {
            if (_context.Candidates.Any(c => c.Username == request.Username))
            {
                return ApiResponse<string>.Failure(400, "Username đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
            }

            var candidate = new Candidate
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Education = request.Education,
                Experience = request.Experience,
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.Now
            };

            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "Hồ sơ ứng viên đã được tạo thành công.");
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
                    Username = c.Username,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<CandidateResponse>>.Success(200, candidates);
        }

        public async Task<ApiResponse<CandidateResponse>> GetCandidateByIdAsync(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
                return ApiResponse<CandidateResponse>.Failure(404, "Ứng viên không tồn tại.");

            var response = new CandidateResponse
            {
                Id = candidate.Id,
                FullName = candidate.FullName,
                Email = candidate.Email,
                Phone = candidate.Phone,
                Education = candidate.Education,
                Experience = candidate.Experience,
                Username = candidate.Username,
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
            candidate.Username = request.Username;
            if (!string.IsNullOrEmpty(request.Password))
            {
                candidate.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

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
