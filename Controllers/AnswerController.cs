using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Dto
{
    [ApiController]
    [Route("api/v1/answers")]
    public class AnswerController : ControllerBase
    {
        private AnswerService _answerService;
        public AnswerController(AnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> post([FromBody] AnswerDTO answerDTO)
        {
            var postAnswer = await _answerService.postAnswer(answerDTO);
            return Ok(postAnswer);
        }

        [HttpGet("get/all/{pageNumber}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAll(int pageNumber)
        {
            var answer = await _answerService.getAllAnswer(pageNumber);
            return Ok(answer);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getByID(int id)
        {
            var answer = await _answerService.getAnswerById(id);
            if (answer.ErrorCode == 404) return NotFound(answer);
            return Ok(answer);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> putAnswer(int id, [FromBody] AnswerDTO answerDTO)
        {
            var answer = await _answerService.updateAnswer(id, answerDTO);
            if (answer.ErrorCode == 404) return NotFound(answer);
            return Ok(answer);
        }

    }
}