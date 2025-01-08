using five_birds_be.Data;
using five_birds_be.DTO;
using five_birds_be.DTO.Request;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public interface ICandidatePositionService
    {
        Task<ApiResponse<List<CandidatePositionResponse>>> GetAllCandidatePositionsAsync();
        Task<ApiResponse<CandidatePositionResponse>> GetCandidatePositionByIdAsync(int id);
        Task<ApiResponse<string>> CreateCandidatePositionAsync(CandidatePositionRequest request);
        Task<ApiResponse<string>> UpdateCandidatePositionAsync(int id, CandidatePositionRequest request);
        Task<ApiResponse<string>> DeleteCandidatePositionAsync(int id);
    }

    public class CandidatePositionService : ICandidatePositionService
    {
        private readonly DataContext _context;

        public CandidatePositionService(DataContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CandidatePositionResponse>>> GetAllCandidatePositionsAsync()
        {
            var candidatePositions = await _context.CandidatePositions
                .Select(cp => new CandidatePositionResponse
                {
                    Id = cp.Id,
                    Name = cp.Name
                })
                .ToListAsync();

            return ApiResponse<List<CandidatePositionResponse>>.Success(200, candidatePositions);
        }

        public async Task<ApiResponse<CandidatePositionResponse>> GetCandidatePositionByIdAsync(int id)
        {
            var candidatePosition = await _context.CandidatePositions
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (candidatePosition == null)
                return ApiResponse<CandidatePositionResponse>.Failure(404, "Không tìm thấy vị trí ứng viên.");

            var response = new CandidatePositionResponse
            {
                Id = candidatePosition.Id,
                Name = candidatePosition.Name
            };

            return ApiResponse<CandidatePositionResponse>.Success(200, response);
        }

       public async Task<ApiResponse<string>> CreateCandidatePositionAsync(CandidatePositionRequest request)
{
    // Kiểm tra nếu tên vị trí đã tồn tại
    if (await _context.CandidatePositions.AnyAsync(cp => cp.Name == request.Name))
    {
        return ApiResponse<string>.Failure(400, "Tên vị trí ứng viên đã tồn tại.");
    }

    // Tạo mới CandidatePosition
    var candidatePosition = new CandidatePosition
    {
        Name = request.Name,
    };

    _context.CandidatePositions.Add(candidatePosition);
    await _context.SaveChangesAsync();

    // Trả về trạng thái thành công với mã 201
    return ApiResponse<string>.Success(201, "Tạo mới vị trí ứng viên thành công.");
}


        public async Task<ApiResponse<string>> UpdateCandidatePositionAsync(int id, CandidatePositionRequest request)
        {
            var candidatePosition = await _context.CandidatePositions.FirstOrDefaultAsync(cp => cp.Id == id);
            if (candidatePosition == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy vị trí ứng viên.");

            candidatePosition.Name = request.Name;
            _context.CandidatePositions.Update(candidatePosition);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Success(200, "Cập nhật vị trí ứng viên thành công.");
        }

        public async Task<ApiResponse<string>> DeleteCandidatePositionAsync(int id)
        {
            var candidatePosition = await _context.CandidatePositions.FirstOrDefaultAsync(cp => cp.Id == id);
            if (candidatePosition == null)
                return ApiResponse<string>.Failure(404, "Không tìm thấy vị trí ứng viên.");

            _context.CandidatePositions.Remove(candidatePosition);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.Success(200, "Xóa vị trí ứng viên thành công.");
        }
    }
}
