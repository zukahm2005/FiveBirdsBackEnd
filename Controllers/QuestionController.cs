using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/questions")]
    public class QuestionController : ControllerBase
    {
        private QuestionService _questionService;

        public QuestionController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> post([FromBody] QuestionDTO questionDTO)
        {
            var question = await _questionService.postQuestion(questionDTO);
            if (question.ErrorCode == 404) return NotFound(question);

            return Ok(question);

        }

        [HttpGet("get/all/{pageNumber}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAll(int pageNumber)
        {
            var question = await _questionService.getALlQuestion(pageNumber);
            return Ok(question);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAnswerById(int id)
        {
            var answer = await _questionService.GetQuestionById(id);
            if (answer.ErrorCode == 404) return NotFound(answer);

            return Ok(answer);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> update(int id, [FromBody] QuestionDTO questionDTO)
        {
            var question = await _questionService.updateQuestion(id, questionDTO);
            if (question.ErrorCode == 404) return NotFound(question);
            return Ok(question);
        }
    }
}