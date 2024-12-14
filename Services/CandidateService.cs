using five_birds_be.Data;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public interface ICandidateService
    {
        Task<string> CreateCandidateAsync(CandidateRequest request);
        Task<List<CandidateResponse>> GetCandidatesAsync();
    }

    public class CandidateService : ICandidateService
    {
        private readonly DataContext _context;

        public CandidateService(DataContext context)
        {
            _context = context;
        }

        public async Task<string> CreateCandidateAsync(CandidateRequest request)
        {
            if (_context.Candidates.Any(c => c.Username == request.Username))
            {
                return "Username đã tồn tại. Vui lòng chọn tên đăng nhập khác.";
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
            return "Hồ sơ ứng viên đã được tạo thành công.";
        }

        public async Task<List<CandidateResponse>> GetCandidatesAsync()
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
                }).ToListAsync();

            return candidates;
        }
    }
}