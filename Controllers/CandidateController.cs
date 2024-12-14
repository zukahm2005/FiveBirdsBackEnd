using five_birds_be.Data;
using five_birds_be.DTO.Request;
using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{

    [ApiController]
    [Route("api/candidates")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        // API tạo hồ sơ ứng viên
        [HttpPost]
        public async Task<IActionResult> CreateCandidate([FromBody] CandidateRequest request)
        {
            var result = await _candidateService.CreateCandidateAsync(request);
            if (result.Contains("đã tồn tại"))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // API lấy danh sách ứng viên
        [HttpGet]
        public async Task<IActionResult> GetCandidates()
        {
            var candidates = await _candidateService.GetCandidatesAsync();
            return Ok(candidates);
        }
    }

}