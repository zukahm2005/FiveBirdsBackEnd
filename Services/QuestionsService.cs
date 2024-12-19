using CloudinaryDotNet;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
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
        public async Task<ApiResponse<Question>> postQuestion(QuestionDTO questionDTO)
        {
            var questionWithExam = await _dataContext.Exam
            .FirstOrDefaultAsync(e => e.Id == questionDTO.ExamId);

            if (questionWithExam == null) return  ApiResponse<Question>.Failure(404, "exam not found");

            var question = await _dataContext.Question
            .FirstOrDefaultAsync(e => e.QuestionExam == questionDTO.QuestionExam && e.ExamId == questionDTO.ExamId);

            if (question != null) return ApiResponse<Question>.Failure(400, "question already existed ");

            var newQuestion = new Question
            {
                ExamId = questionDTO.ExamId,
                Exam = questionWithExam,
                QuestionExam = questionDTO.QuestionExam,
                Point = questionDTO.Point,
            };
            await _dataContext.Question.AddAsync(newQuestion);
            await _dataContext.SaveChangesAsync();
            return  ApiResponse<Question>.Success(200, newQuestion,  "create success");
        }

        public async Task<ApiResponse<List<Question>>> getALlQuestion(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            var pageQuestion = await _dataContext.Question
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ToListAsync();

            return ApiResponse<List<Question>>.Success(200, pageQuestion, "get all success");
        }

        public async Task<ApiResponse<Question>> GetQuestionById(int id){
            var question = await _dataContext.Question
            .Include(exam => exam.Exam)
            .FirstOrDefaultAsync(q => q.Id == id); 

            if (question == null) return ApiResponse<Question>.Failure(400, "id question not found");

            return ApiResponse<Question>.Success(200, question, "get by id question success");
        }

        public async Task<ApiResponse<QuestionResponse>> updateQuestion(int id, QuestionDTO questionDTO){
            var question = await _dataContext.Question.FindAsync(id);
            if (question == null) return ApiResponse<QuestionResponse>.Failure(400, "id question not found");

            question.ExamId = questionDTO.ExamId;
            question.QuestionExam = questionDTO.QuestionExam;
            question.Point = questionDTO.Point;
            question.Update_at = DateTime.Now;

            var newQuestionDTO = new QuestionResponse{
                ExamId = question.ExamId,
                QuestionExam = question.QuestionExam,
                Point = questionDTO.Point,
            };

            await _dataContext.SaveChangesAsync();

            return ApiResponse<QuestionResponse>.Success(200, newQuestionDTO, "update question success");
        }
    }
}