using five_birds_be.DTO.Request;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/candidates")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateCandidate([FromForm] CandidateRequest request)
        {
            var response = await _candidateService.CreateCandidateAsync(request);
            if (response.ErrorCode != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCandidates()
        {
            var response = await _candidateService.GetCandidatesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidateById(int id)
        {
            var response = await _candidateService.GetCandidateByIdAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidate(int id, [FromBody] CandidateRequest request)
        {
            var response = await _candidateService.UpdateCandidateAsync(id, request);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var response = await _candidateService.DeleteCandidateAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }
    }
}
