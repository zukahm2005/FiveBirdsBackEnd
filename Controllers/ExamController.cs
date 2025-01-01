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

        [HttpGet("get/all")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getAll(int pageNumber, int pageSize)
        {
            var exam = await _examService.getAllExam(pageNumber, pageSize);
            if (exam == null) return NotFound(ApiResponse<Exam>.Failure(404, "No exam found"));
            return Ok(exam);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<IActionResult> getById(int id)
        {
            var exam = await _examService.getExamById(id);
            if (exam.ErrorCode == 404) return NotFound(exam);
            return Ok(exam);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> putExam(int id, [FromBody] ExamDTO examDTO)
        {
            var exam = await _examService.updateExam(id, examDTO);
            if (exam.ErrorCode == 404) return NotFound(exam);
            return Ok(exam);
        }
        [HttpGet("get")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> getExam()
        {
            var exam = await _examService.getExam();
            return Ok(exam);
        }
    }
}