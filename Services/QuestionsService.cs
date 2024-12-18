using CloudinaryDotNet;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class QuestionService
    {
        private DataContext _dataContext;

        public QuestionService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ApiResponse<string>> postQuestion(QuestionDTO questionDTO)
        {
            var questionWithExam = await _dataContext.Exam
            .FirstOrDefaultAsync(e => e.Id == questionDTO.ExamId);

            if (questionWithExam == null) return new ApiResponse<string>(404, "exam not found", null);

            Console.WriteLine("sdfs" + questionWithExam);


            var newQuestion = new Question
            {
                ExamId = questionDTO.ExamId,
                Exam = questionWithExam,
                QuestionExam = questionDTO.QuestionExam,
                Point = questionDTO.Point,
            };
            await _dataContext.Question.AddAsync(newQuestion);
            await _dataContext.SaveChangesAsync();
            return new ApiResponse<string>(200, "create susscess", null);
        }

        public async Task<List<Question>> getALlQuestion(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Question
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ToListAsync();
        }

        public async Task<ApiResponse<Question>> GetQuestionById(int id){
            var question = await _dataContext.Question
            .Include(exam => exam.Exam)
            .FirstOrDefaultAsync(q => q.Id == id); 

            if (question == null) return ApiResponse<Question>.Failure(400, "id question not found");

            return ApiResponse<Question>.Success(200, question, "get by id question success");
        }

        public async Task<ApiResponse<QuestionDTO>> updateQuestion(int id, QuestionDTO questionDTO){
            var question = await _dataContext.Question.FindAsync(id);
            if (question == null) return ApiResponse<QuestionDTO>.Failure(400, "id question not found");

            question.ExamId = questionDTO.ExamId;
            question.QuestionExam = questionDTO.QuestionExam;
            question.Point = questionDTO.Point;

            var newQuestionDTO = new QuestionDTO{
                ExamId = question.ExamId,
                QuestionExam = question.QuestionExam,
                Point = questionDTO.Point,
            };

            await _dataContext.SaveChangesAsync();

            return ApiResponse<QuestionDTO>.Success(200, newQuestionDTO, "update question success");
        }
    }
}