using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.IO;
using five_birds_be.DTO;

namespace five_birds_be.Services
{
    public class GoogleSheetService
    {
        private const string ApplicationName = "Five Birds Backend";
        private const string CredentialPath = "credentials.json"; // Tải file credentials.json từ Google Cloud Console

        public List<QuestionImportDTO> ReadQuestionsFromGoogleSheet(string sheetId, string range)
        {
            var credential = GoogleCredential.FromFile(CredentialPath)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            var request = service.Spreadsheets.Values.Get(sheetId, range);
            var response = request.Execute();
            var values = response.Values;

            var questions = new List<QuestionImportDTO>();

            if (values == null || values.Count == 0) return questions;

            foreach (var row in values)
            {
                var question = new QuestionImportDTO
                {
                    ExamId = int.Parse(row[0].ToString()),
                    QuestionText = row[1].ToString(),
                    Point = int.Parse(row[2].ToString()),
                    Answer1 = row[3].ToString(),
                    Answer2 = row[4].ToString(),
                    Answer3 = row[5].ToString(),
                    Answer4 = row[6].ToString(),
                    CorrectAnswer = int.Parse(row[7].ToString())
                };

                questions.Add(question);
            }

            return questions;
        }
    }
}
