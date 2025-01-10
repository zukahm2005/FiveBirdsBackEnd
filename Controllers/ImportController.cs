using Microsoft.AspNetCore.Mvc;
using five_birds_be.Services;
using five_birds_be.Models;
using five_birds_be.Dto;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using five_birds_be.Data;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ExcelService _excelService;
        private readonly GoogleSheetService _googleSheetService;

        public ImportController(DataContext context, ExcelService excelService, GoogleSheetService googleSheetService)
        {
            _context = context;
            _excelService = excelService;
            _googleSheetService = googleSheetService;
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            var questions = _excelService.ReadQuestionsFromExcel(stream);

            foreach (var questionDto in questions)
            {
                var question = new Question
                {
                    ExamId = questionDto.ExamId,
                    QuestionExam = questionDto.QuestionText,
                    Point = questionDto.Point,
                    Answers = new List<Answer>
                    {
                        new Answer
                        {
                            Answer1 = questionDto.Answer1,
                            Answer2 = questionDto.Answer2,
                            Answer3 = questionDto.Answer3,
                            Answer4 = questionDto.Answer4,
                            CorrectAnswer = questionDto.CorrectAnswer
                        }
                    }
                };

                _context.Question.Add(question);
            }

            await _context.SaveChangesAsync();
            return Ok("Import thành công từ Excel!");
        }

        [HttpPost("google-sheet")]
        public async Task<IActionResult> ImportFromGoogleSheet([FromBody] string sheetId)
        {
            if (string.IsNullOrEmpty(sheetId))
                return BadRequest("Google Sheet ID không hợp lệ.");

            var range = "Sheet1!A2:H";
            var questions = _googleSheetService.ReadQuestionsFromGoogleSheet(sheetId, range);

            foreach (var questionDto in questions)
            {
                var question = new Question
                {
                    ExamId = questionDto.ExamId,
                    QuestionExam = questionDto.QuestionText,
                    Point = questionDto.Point,
                    Answers = new List<Answer>
                    {
                        new Answer
                        {
                            Answer1 = questionDto.Answer1,
                            Answer2 = questionDto.Answer2,
                            Answer3 = questionDto.Answer3,
                            Answer4 = questionDto.Answer4,
                            CorrectAnswer = questionDto.CorrectAnswer
                        }
                    }
                };

                _context.Question.Add(question);
            }

            await _context.SaveChangesAsync();
            return Ok("Import thành công từ Google Sheets!");
        }
    }
}
