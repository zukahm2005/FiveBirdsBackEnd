using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Servi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/exams")]
    public class ExamController : ControllerBase
    {
        private ExamService _examService;
        public ExamController(ExamService examService)
        {
            _examService = examService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> createExam([FromBody] ExamDTO examDTO)
        {
            var exam = await _examService.CreateExam(examDTO);
            return Ok(exam);
        }

        [HttpGet("get/all/{pageNumber}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAll(int pageNumber)
        {
            var exam = await _examService.getAllExam(pageNumber);
            if (exam == null) return NotFound(ApiResponse<Exam>.Failure(404, "No exam found"));
            return Ok(exam);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<IActionResult> getById(int Id)
        {
            var exam = await _examService.getExamById(Id);
            if (exam.ErrorCode == 404) return NotFound(exam);
            return Ok(exam);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> putExam(int Id, [FromBody] ExamDTO examDTO)
        {
            var exam = await _examService.updateExam(Id, examDTO);
            if (exam.ErrorCode == 404) return NotFound(exam);
            return Ok(exam);
        }
    }
}