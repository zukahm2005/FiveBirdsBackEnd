using five_birds_be.DTO;
using five_birds_be.DTO.Request;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/candidate-positions")]
    public class CandidatePositionController : ControllerBase
    {
        private readonly ICandidatePositionService _candidatePositionService;

        public CandidatePositionController(ICandidatePositionService candidatePositionService)
        {
            _candidatePositionService = candidatePositionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCandidatePositions()
        {
            var response = await _candidatePositionService.GetAllCandidatePositionsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidatePositionById(int id)
        {
            var response = await _candidatePositionService.GetCandidatePositionByIdAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

       [HttpPost]
public async Task<IActionResult> CreateCandidatePosition([FromBody] CandidatePositionRequest request)
{
    var response = await _candidatePositionService.CreateCandidatePositionAsync(request);

    // Kiểm tra trạng thái trả về từ service
    if (response.ErrorCode == 400)
        return BadRequest(response);

    // Trả về mã trạng thái 201 khi thành công
    if (response.ErrorCode == 201)
        return StatusCode(201, response);

    return Ok(response); // Dành cho các trường hợp khác (nếu cần)
}


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidatePosition(int id, [FromBody] CandidatePositionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure(400, "Dữ liệu không hợp lệ."));

            var response = await _candidatePositionService.UpdateCandidatePositionAsync(id, request);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidatePosition(int id)
        {
            var response = await _candidatePositionService.DeleteCandidatePositionAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }
    }
}
