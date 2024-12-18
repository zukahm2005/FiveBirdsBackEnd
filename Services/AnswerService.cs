using CloudinaryDotNet;
using five_birds_be.Data;
using five_birds_be.Dto;
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

        public async Task<ApiResponse<string>> postAnswer(AnswerDTO answerDTO)
        {
            var question = await _dataContext.Question
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == answerDTO.QuestionId);
            if (question == null) { }

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
            return new ApiResponse<string>(200, "create susscess", null);
        }
        public async Task<List<Answer>> getAllAnswer(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Answer
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ToListAsync();
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

            var newAnswerDto = new AnswerDTO
            {
                Id = answer.Id,
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
    }
}