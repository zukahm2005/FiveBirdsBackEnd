using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class CandidateTestService
    {
        public DataContext _dataContext;
        public CandidateTestService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<CandidateTestRespone>> addCandidateTest(CandidateTestDTO candidateTest)
        {

            var dataUser = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == candidateTest.UserId);
            if (dataUser == null) return ApiResponse<CandidateTestRespone>.Failure(400, "fsdfsdf");

            var dataExam = await _dataContext.Exam.FirstOrDefaultAsync(u => u.Id == candidateTest.ExamId);
            if (dataExam == null) return ApiResponse<CandidateTestRespone>.Failure(400, "fsdfsdf");

            var query = _dataContext.Result
               .Where(r => r.ExamId == candidateTest.ExamId && r.UserId == candidateTest.UserId);

            var dataResults = await query.ToListAsync();

            var newPoint = 0;

            foreach (var result in dataResults)
            {
                if (result.Is_correct == true && result.QuestionId != null)
                {
                    var questionData = await _dataContext.Question.FirstOrDefaultAsync(q => q.Id == result.QuestionId);
                    if (questionData != null) // Check for null
                    {
                        Console.WriteLine($"sndlkang: {questionData.Point}");
                        newPoint += Convert.ToInt32(questionData.Point); // Add the points
                    }
                    else
                    {
                        Console.WriteLine($"Question not found for QuestionId: {result.QuestionId}");
                    }
                }
            }
            if (newPoint >= 80)
            {
                candidateTest.IsPast = true;
            }
            else
            {
                candidateTest.IsPast = false;
            }

            var newCandidateTest = new CandidateTest
            {
                Id = candidateTest.Id,
                UserId = candidateTest.UserId,
                User = dataUser,
                ExamId = candidateTest.ExamId,
                Exam = dataExam,
                Point = newPoint,
                IsPast = candidateTest.IsPast
            };

            var newCandidateTestResponse = new CandidateTestRespone
            {
                Id = newCandidateTest.Id,
                UserId = newCandidateTest.UserId,
                ExamId = newCandidateTest.ExamId,
                Point = newCandidateTest.Point,
                IsPast = newCandidateTest.IsPast

            };

            await _dataContext.CandidateTests.AddAsync(newCandidateTest);
            await _dataContext.SaveChangesAsync();


            return ApiResponse<CandidateTestRespone>.Success(200, newCandidateTestResponse, "create success");

        }
        
    }
}