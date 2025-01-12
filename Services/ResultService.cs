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

        public async Task<ApiResponse<List<ResultResponse>>> PostResults(List<ResultDTO> resultDTOs)
        {
            using var transaction = await _dataContext.Database.BeginTransactionAsync();

            try
            {
                var resultResponses = new List<ResultResponse>();

                var userIds = resultDTOs.Select(r => r.UserId).Distinct();
                var examIds = resultDTOs.Select(r => r.ExamId).Distinct();
                var questionIds = resultDTOs.Select(r => r.QuestionId).Distinct();
                var answerIds = resultDTOs.Select(r => r.AnswerId).Distinct();

                var existingResults = await _dataContext.Result
                    .Where(r => userIds.Contains(r.UserId) && examIds.Contains(r.ExamId) && questionIds.Contains(r.QuestionId))
                    .ToListAsync();

                var users = await _dataContext.User.Where(u => userIds.Contains(u.UserId)).ToListAsync();
                var exams = await _dataContext.Exam.Where(e => examIds.Contains(e.Id)).ToListAsync();
                var questions = await _dataContext.Question.Where(q => questionIds.Contains(q.Id)).ToListAsync();
                var answers = await _dataContext.Answer.Where(a => answerIds.Contains(a.Id)).ToListAsync();

                foreach (var resultDTO in resultDTOs)
                {
                    if (existingResults.Any(r => r.UserId == resultDTO.UserId && r.ExamId == resultDTO.ExamId && r.QuestionId == resultDTO.QuestionId))
                    {
                        throw new Exception($"User has already answered question ID: {resultDTO.QuestionId}.");
                    }

                    var user = users.FirstOrDefault(u => u.UserId == resultDTO.UserId);
                    if (user == null) throw new Exception($"Invalid UserId: {resultDTO.UserId}.");

                    var exam = exams.FirstOrDefault(e => e.Id == resultDTO.ExamId);
                    if (exam == null) throw new Exception($"Invalid ExamId: {resultDTO.ExamId}.");

                    var question = questions.FirstOrDefault(q => q.Id == resultDTO.QuestionId);
                    if (question == null) throw new Exception($"Invalid QuestionId: {resultDTO.QuestionId}.");

                    var answer = answers.FirstOrDefault(a => a.Id == resultDTO.AnswerId);
                    if (answer == null) throw new Exception($"Invalid AnswerId: {resultDTO.AnswerId}.");

                    bool correct = answer.CorrectAnswer == resultDTO.ExamAnswer;

                    var newResult = new Result
                    {
                        UserId = resultDTO.UserId,
                        User = user,
                        ExamId = resultDTO.ExamId,
                        Exam = exam,
                        QuestionId = resultDTO.QuestionId,
                        Questions = question,
                        AnswerId = resultDTO.AnswerId,
                        Answer = answer,
                        ExamAnswer = resultDTO.ExamAnswer,
                        Is_correct = correct
                    };

                    await _dataContext.Result.AddAsync(newResult);

                    resultResponses.Add(new ResultResponse
                    {
                        Id = newResult.Id,
                        UserId = newResult.UserId,
                        ExamId = newResult.ExamId,
                        QuestionId = newResult.QuestionId,
                        AnswerId = newResult.AnswerId,
                        ExamAnswer = newResult.ExamAnswer,
                        Is_correct = newResult.Is_correct
                    });
                }

                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResponse<List<ResultResponse>>.Success(200, resultResponses, "All results created successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<List<ResultResponse>>.Failure(400, ex.Message);
            }
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

            return ApiResponse<object>.Success(200, results, "get all success");
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
        public async Task<ApiResponse<ResultResponse>> updateResult(int id, ResultDTO resultDTO)
        {
            var data = await _dataContext.Result.FirstOrDefaultAsync(r => r.Id == id);
            if (data == null) return ApiResponse<ResultResponse>.Failure(404, "result not found");

            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == resultDTO.UserId);
            if (user == null) return ApiResponse<ResultResponse>.Failure(404, "user id not found");

            var answers = await _dataContext.Answer.FirstOrDefaultAsync(a => a.Id == resultDTO.AnswerId);
            if (answers == null) return ApiResponse<ResultResponse>.Failure(404, "answer id not found");

            var question = await _dataContext.Question.FirstOrDefaultAsync(a => a.Id == resultDTO.QuestionId);
            if (question == null) return ApiResponse<ResultResponse>.Failure(404, "question id not found");

            data.UserId = resultDTO.UserId;
            data.AnswerId = resultDTO.AnswerId;
            data.QuestionId = resultDTO.QuestionId;
            data.AnswerId = resultDTO.ExamAnswer;

            await _dataContext.SaveChangesAsync();

            var newData = new ResultResponse
            {
                Id = data.Id,
                UserId = data.UserId,
                ExamId = data.ExamId,
                QuestionId = data.QuestionId,
                AnswerId = data.AnswerId,
                ExamAnswer = data.ExamAnswer,
                Is_correct = data.Is_correct
            };
            return ApiResponse<ResultResponse>.Success(200, newData);
        }

        public async Task<ApiResponse<string>> deleteResult(int id)
        {
            var data = await _dataContext.Result.FirstOrDefaultAsync(r => r.Id == id);
            if (data == null) return ApiResponse<string>.Failure(404, "result not found");

            _dataContext.Result.Remove(data);
            await _dataContext.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "delete result success");
        }

    }
}