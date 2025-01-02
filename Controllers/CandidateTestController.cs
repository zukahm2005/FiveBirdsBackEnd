using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/candidate/test")]
    public class CandidateTestController : ControllerBase
    {
        private CandidateTestService _candidateTestService;

        public CandidateTestController(CandidateTestService candidateTestService)
        {
            _candidateTestService = candidateTestService;
        }


        [HttpPost("add")]
        [Authorize(Roles = "ROLE_CANDIDATE")]
        public async Task<IActionResult> post(CandidateTestDTO candidateTest)
        {
            var data = await _candidateTestService.addCandidateTest(candidateTest);
            if (data.ErrorCode == 400) return BadRequest(data);
            return Ok(data);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _candidateTestService.GetCandidateTestResultByIdAsync(id);
            if (data.ErrorCode == 404) return NotFound(data);
            return Ok(data);
        }

        [HttpGet("get/all")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAll(int pageNumber, int pageSize)
        {
            var data = await _candidateTestService.GetAll(pageNumber, pageSize);
            return Ok(data);
        }
    }
}