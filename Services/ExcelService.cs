using OfficeOpenXml;
using five_birds_be.Dto;
using System.Collections.Generic;
using System.IO;
using five_birds_be.DTO;

namespace five_birds_be.Services
{
    public class ExcelService
    {
        public List<QuestionImportDTO> ReadQuestionsFromExcel(Stream fileStream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(fileStream);

            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension.Rows;

            var questions = new List<QuestionImportDTO>();

            for (int row = 2; row <= rowCount; row++)
            {
                var question = new QuestionImportDTO
                {
                    ExamId = int.Parse(worksheet.Cells[row, 1].Text),
                    QuestionText = worksheet.Cells[row, 2].Text,
                    Point = int.Parse(worksheet.Cells[row, 3].Text),
                    Answer1 = worksheet.Cells[row, 4].Text,
                    Answer2 = worksheet.Cells[row, 5].Text,
                    Answer3 = worksheet.Cells[row, 6].Text,
                    Answer4 = worksheet.Cells[row, 7].Text,
                    CorrectAnswer = int.Parse(worksheet.Cells[row, 8].Text)
                };

                questions.Add(question);
            }

            return questions;
        }
    }
}
