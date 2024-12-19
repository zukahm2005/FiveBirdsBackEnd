using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Dto{
    [ApiController]
    [Route("api/v1/answers")]
    public class AnswerController: ControllerBase{
        private AnswerService _answerService;
        public AnswerController(AnswerService answerService){
            _answerService = answerService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> post([FromBody] AnswerDTO answerDTO){
            var postAnswer = await _answerService.postAnswer(answerDTO);
            return Ok(postAnswer);
        }

        [HttpGet("get/all/{pageNumber}")]
        public async Task<IActionResult> getAll (int pageNumber){
            var answer = await _answerService.getAllAnswer(pageNumber);
            return Ok(answer);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> getByID(int id){
            var answer = await _answerService.getAnswerById(id);
            if(answer.ErrorCode == 404) return NotFound(answer);
            return Ok(answer);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> putAnswer(int id, [FromBody] AnswerDTO answerDTO){
            var answer = await _answerService.updateAnswer(id, answerDTO);
            if(answer.ErrorCode == 404) return NotFound(answer);
            return Ok(answer);
        }

    }
}