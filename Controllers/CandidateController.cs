using five_birds_be.DTO.Request;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
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
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure(400, "Dữ liệu không hợp lệ."));

            var response = await _candidateService.CreateCandidateAsync(request);
            if (response.ErrorCode != 200)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetCandidates()
        {
            var response = await _candidateService.GetCandidatesAsync();
            return Ok(response);
        }

        [HttpGet("get/all")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetCandidatesPage(int pageNumber, int pageSize, StatusEmail? statusEmail, int? CandidatePositionId,  DateTime? startDate, DateTime? endDate)
        {
            var response = await _candidateService.GetCandidatesPage(pageNumber, pageSize, statusEmail, CandidatePositionId, startDate, endDate);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<IActionResult> GetCandidateById(int id)
        {
            // Kiểm tra quyền truy cập nếu người dùng không phải ROLE_ADMIN
            if (!User.IsInRole("ROLE_ADMIN"))
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != id)
                    return Forbid();
            }

            var response = await _candidateService.GetCandidateByIdAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateCandidate(int id, [FromForm] CandidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure(400, "Dữ liệu không hợp lệ."));

            var response = await _candidateService.UpdateCandidateAsync(id, request);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var response = await _candidateService.DeleteCandidateAsync(id);
            if (response.ErrorCode != 200)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost("send/email/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> SendEmailCandidate(int id, [FromBody] EmailRequest emailRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Failure(400, "Dữ liệu không hợp lệ."));

            var response = await _candidateService.SendEmailCandidate(id, emailRequest);
            if (response.ErrorCode == 404)
                return NotFound(response);

            if (response.ErrorCode != 200)
                return StatusCode(500, response);

            return Ok(response);
        }
        [HttpPost("send/email/interview")]
        public async Task<IActionResult> SendEmailInterview(string email, string interviewDate){
            var response = await _candidateService.SendEmailInterviewSchedule(email, interviewDate);
            return Ok(response);
        }
    }
}
