using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/candidate/test")]
    public class CandidateTestController : ControllerBase
    {
        private CandidateTestService _candidateTestService;

        public CandidateTestController(CandidateTestService candidateTestService){
            _candidateTestService = candidateTestService;
        }
         

        [HttpPost("add")]
        public async Task<IActionResult> post(CandidateTestDTO candidateTest)
        {
            var data = await _candidateTestService.addCandidateTest(candidateTest);
            return Ok(data);
        }
    }
}