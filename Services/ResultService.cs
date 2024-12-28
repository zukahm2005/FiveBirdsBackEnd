using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class ResultService
    {
        private readonly DataContext _dataContext;

        public ResultService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<ResultResponse>> PostResult(ResultDTO resultDTO)
        {
            var dataUser = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == resultDTO.UserId);
            if (dataUser == null) return ApiResponse<ResultResponse>.Failure(400, "Invalid dataUser provided.");

            var dataExam = await _dataContext.Exam.FirstOrDefaultAsync(u => u.Id == resultDTO.ExamId);
            if (dataExam == null) return ApiResponse<ResultResponse>.Failure(400, "Invalid dataExam provided.");

            var dataQuestion = await _dataContext.Question.FirstOrDefaultAsync(u => u.Id == resultDTO.QuestionId);
            if (dataQuestion == null) return ApiResponse<ResultResponse>.Failure(400, "Invalid dataQuestion provided.");

            var dataAnswer = await _dataContext.Answer.FirstOrDefaultAsync(u => u.Id == resultDTO.AnswerId);
            if (dataAnswer == null) return ApiResponse<ResultResponse>.Failure(400, "Invalid dataAnswer provided.");

            bool correct = dataAnswer.CorrectAnswer == resultDTO.ExamAnswer;

            var newResult = new Result
            {
                UserId = resultDTO.UserId,
                User = dataUser,
                ExamId = resultDTO.ExamId,
                Exam = dataExam,
                QuestionId = resultDTO.QuestionId,
                Questions = dataQuestion,
                AnswerId = resultDTO.AnswerId,
                Answer = dataAnswer,
                ExamAnswer = resultDTO.ExamAnswer,
                Is_correct = correct
            };

            var ResultResponse = new ResultResponse
            {
                Id = newResult.Id,
                UserId = newResult.UserId,
                ExamId = newResult.ExamId,
                QuestionId = newResult.QuestionId,
                AnswerId = newResult.AnswerId,
                ExamAnswer = newResult.ExamAnswer,
                Is_correct = newResult.Is_correct
            };

            await _dataContext.Result.AddAsync(newResult);
            await _dataContext.SaveChangesAsync();

            return ApiResponse<ResultResponse>.Success(200, ResultResponse, "Create result success");
        }

        public async Task<ApiResponse<object>> GetAll(int pageNumber, int pageSize)
        {
            var results = await _dataContext.Result
                .Include(r => r.User)
                .Include(r => r.Exam)
                .Include(r => r.Questions)
                .Include(r => r.Answer)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    resultId = r.Id,
                    user = new
                    {
                        id = r.User.UserId,
                        name = r.User.UserName,
                        email = r.User.Email,
                    },
                    exam = new
                    {
                        id = r.Exam.Id,
                        title = r.Exam.Title,
                        description = r.Exam.Description,
                        duration = r.Exam.Duration
                    },
                    question = new
                    {
                        id = r.Questions.Id,
                        text = r.Questions.QuestionExam,
                        point = r.Questions.Point,
                        answers = r.Questions.Answers.Select(a => new
                        {
                            id = a.Id,
                            text = a.Answer2,
                            isCorrect = a.CorrectAnswer
                        })
                    },
                    userAnswer = r.ExamAnswer,
                    isCorrect = r.Is_correct
                })
                .ToListAsync();

            return ApiResponse<object>.Success(200, results, "create success");
        }

        public async Task<ApiResponse<object>> GetById(int resultId)
        {
            var result = await _dataContext.Result
                .Include(r => r.User)
                .Include(r => r.Exam)
                .Include(r => r.Questions)
                .Include(r => r.Answer)
                .Where(r => r.Id == resultId)
                .Select(r => new
                {
                    resultId = r.Id,
                    user = new
                    {
                        id = r.User.UserId,
                        name = r.User.UserName,
                        email = r.User.Email,
                    },
                    exam = new
                    {
                        id = r.Exam.Id,
                        title = r.Exam.Title,
                        description = r.Exam.Description,
                        duration = r.Exam.Duration
                    },
                    question = new
                    {
                        id = r.Questions.Id,
                        text = r.Questions.QuestionExam,
                        point = r.Questions.Point,
                        answers = r.Questions.Answers.Select(a => new
                        {
                            id = a.Id,
                            text = a.Answer2,
                            isCorrect = a.CorrectAnswer
                        })
                    },
                    userAnswer = r.ExamAnswer,
                    isCorrect = r.Is_correct
                })
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return ApiResponse<object>.Failure(404, "Result not found");
            }

            return ApiResponse<object>.Success(200, result, "Retrieve success");
        }

    }
}
