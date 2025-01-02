using CloudinaryDotNet;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class AnswerService
    {
        private DataContext _dataContext;

        public AnswerService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<AnswerResponse>> postAnswer(AnswerDTO answerDTO)
        {
            var question = await _dataContext.Question
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == answerDTO.QuestionId);
            if (question == null) return ApiResponse<AnswerResponse>.Failure(404, "id not found");

            var existingAnswer = question.Answers.FirstOrDefault();
            if (existingAnswer != null)
                return ApiResponse<AnswerResponse>.Failure(400, "Answer for this question already exists");

            var newAnswer = new Answer
            {
                QuestionId = answerDTO.QuestionId,
                Question = question,
                Answer1 = answerDTO.Answer1,
                Answer2 = answerDTO.Answer2,
                Answer3 = answerDTO.Answer3,
                Answer4 = answerDTO.Answer4,
                CorrectAnswer = answerDTO.CorrectAnswer,
            };

            await _dataContext.Answer.AddAsync(newAnswer);
            await _dataContext.SaveChangesAsync();

            var newAnswerResponse = new AnswerResponse
            {
                Id = newAnswer.Id,
                QuestionId = newAnswer.QuestionId,
                QuestionExam = newAnswer.Question.QuestionExam,
                Answer1 = newAnswer.Answer1,
                Answer2 = newAnswer.Answer2,
                Answer3 = newAnswer.Answer3,
                Answer4 = newAnswer.Answer4,
                CorrectAnswer = newAnswer.CorrectAnswer,
            };

            return ApiResponse<AnswerResponse>.Success(200, newAnswerResponse, "create susscess");
        }
        public async Task<ApiResponse<List<Answer>>> getAllAnswer(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;

            var pageAnswer = await _dataContext.Answer
            .Skip((pageNumber - 1) * 10)
            .Take(pageSize)
            .ToListAsync();

            return ApiResponse<List<Answer>>.Success(200, pageAnswer, "get all success");
        }

        public async Task<ApiResponse<Answer>> getAnswerById(int id)
        {
            var answer = await _dataContext.Answer.FindAsync(id);
            if (answer == null) return ApiResponse<Answer>.Failure(404, "id answer not found");

            return ApiResponse<Answer>.Success(200, answer, "get by id answer success");

        }

        public async Task<ApiResponse<AnswerDTO>> updateAnswer(int id, AnswerDTO answerDTO)
        {
            var answer = await _dataContext.Answer.FindAsync(id);
            if (answer == null) return ApiResponse<AnswerDTO>.Failure(400, "id answer not found");

            answer.QuestionId = answerDTO.QuestionId;
            answer.Answer1 = answerDTO.Answer1;
            answer.Answer2 = answerDTO.Answer2;
            answer.Answer3 = answerDTO.Answer3;
            answer.Answer4 = answerDTO.Answer4;
            answer.CorrectAnswer = answerDTO.CorrectAnswer;
            answer.Update_at = DateTime.Now;

            var newAnswerDto = new AnswerDTO
            {
                QuestionId = answer.QuestionId,
                Answer1 = answer.Answer1,
                Answer2 = answer.Answer2,
                Answer3 = answer.Answer3,
                Answer4 = answer.Answer4,
                CorrectAnswer = answer.CorrectAnswer
            };

            await _dataContext.SaveChangesAsync();

            return ApiResponse<AnswerDTO>.Success(200, newAnswerDto, "update answer success");
        }

        public async Task<ApiResponse<string>> deleteAnswer(int id){
            var answer = await _dataContext.Answer.FirstOrDefaultAsync(a => a.Id == id);
            if (answer == null) return ApiResponse<string>.Failure(404, "id answer not found");
            _dataContext.Answer.Remove(answer);
            await _dataContext.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "delete answer success");
        }
    }
}