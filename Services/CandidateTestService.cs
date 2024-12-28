using CloudinaryDotNet;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.AspNetCore.Mvc;
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

            var dataCandidateTest = await _dataContext.CandidateTests.FirstOrDefaultAsync(c => c.UserId == candidateTest.UserId);
            if (dataCandidateTest != null) return ApiResponse<CandidateTestRespone>.Failure(400, "has answered");


            var dataUser = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == candidateTest.UserId);
            if (dataUser == null) return ApiResponse<CandidateTestRespone>.Failure(400, "Invalid dataUser provided ");

            var dataExam = await _dataContext.Exam.FirstOrDefaultAsync(u => u.Id == candidateTest.ExamId);
            if (dataExam == null) return ApiResponse<CandidateTestRespone>.Failure(400, "Invalid dataExam provided ");

            var query = _dataContext.Result
               .Where(r => r.ExamId == candidateTest.ExamId && r.UserId == candidateTest.UserId);

            var dataResults = await query.ToListAsync();

            var newPoint = 0;

            foreach (var result in dataResults)
            {
                if (result.Is_correct == true && result.QuestionId != null)
                {
                    var questionData = await _dataContext.Question.FirstOrDefaultAsync(q => q.Id == result.QuestionId);
                    if (questionData != null)
                    {
                        newPoint += Convert.ToInt32(questionData.Point);
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

        private async Task<List<object>> GetResultsByQuestionIdAsync(int questionId, int userId)
        {
            return await _dataContext.Result
                .Where(r => r.QuestionId == questionId && r.UserId == userId)
                .Select(r => new
                {
                    isCorrect = r.Is_correct,
                    selectedAnswer = r.ExamAnswer
                })
                .Cast<object>()
                .ToListAsync();
        }
        public async Task<ApiResponse<object>> GetCandidateTestResultByIdAsync(int id)
        {
            var ct = await _dataContext.CandidateTests
                .Include(ct => ct.User)
                .Include(ct => ct.Exam)
                .ThenInclude(exam => exam.Question)
                .ThenInclude(question => question.Answers)
                .FirstOrDefaultAsync(ct => ct.Id == id);

            if (ct == null)
            {
                return ApiResponse<object>.Failure(404, "Candidate test result not found.");
            }

            var questions = new List<object>();

            foreach (var question in ct.Exam.Question)
            {
                var answers = question.Answers.Select(a => new
                {
                    id = a.Id,
                    answer1 = a.Answer1,
                    answer2 = a.Answer2,
                    answer3 = a.Answer3,
                    answer4 = a.Answer4,
                    isCorrect = a.CorrectAnswer
                }).ToList();

                var questionResults = await GetResultsByQuestionIdAsync(question.Id, ct.UserId);

                questions.Add(new
                {
                    id = question.Id,
                    text = question.QuestionExam,
                    point = question.Point,
                    answers,
                    results = questionResults
                });
            }

            var result = new
            {
                user = new
                {
                    id = ct.User.UserId,
                    name = ct.User.UserName,
                    email = ct.User.Email,
                },
                exam = new
                {
                    id = ct.Exam.Id,
                    title = ct.Exam.Title,
                    description = ct.Exam.Description,
                    duration = ct.Exam.Duration,
                    questions
                },
                ExamId = ct.ExamId,
                Point = ct.Point,
                IsPast = ct.IsPast
            };

            return ApiResponse<object>.Success(200, result, "Result retrieved successfully.");
        }


        public async Task<ApiResponse<object>> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                var data = await _dataContext.CandidateTests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return ApiResponse<object>.Success(200, data, "Get all success");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.Failure(500, $"Error: {ex.Message}");
            }
        }


    }
}