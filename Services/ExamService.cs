using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Servi
{
    public class ExamService
    {
        private DataContext _dataContext;

        public ExamService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<ApiResponse<Exam>> CreateExam(ExamDTO examDTO)
        {
            var newExam = new Exam
            {
                Title = examDTO.Title,
                Description = examDTO.Description,
                Duration = examDTO.Duration,
            };

            await _dataContext.Exam.AddAsync(newExam);
            await _dataContext.SaveChangesAsync();

            return ApiResponse<Exam>.Success(200, newExam, "create success");
        }

        public async Task<ApiResponse<List<ExamResponse>>> getAllExam(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;

            var pageExam = await _dataContext.Exam
            .Include(exam => exam.Question)
            .ThenInclude(question => question.Answers)
            .Skip((pageNumber - 1) * 10)
            .Take(pageSize)
            .ToListAsync();

            var examResponses = pageExam.Select(exam => new ExamResponse
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                Duration = exam.Duration,
                Question = exam.Question.Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    ExamId = q.ExamId,
                    QuestionExam = q.QuestionExam,
                    Point = q.Point,
                    Answers = q.Answers.Select(a => new AnswerResponse
                    {
                        Id = a.Id,
                        QuestionId = a.QuestionId,
                        QuestionExam = a.Question.QuestionExam,
                        Answer1 = a.Answer1,
                        Answer2 = a.Answer2,
                        Answer3 = a.Answer3,
                        Answer4 = a.Answer4,
                        CorrectAnswer = a.CorrectAnswer
                    }).ToList()
                }).ToList()
            }).ToList();

            return ApiResponse<List<ExamResponse>>.Success(200, examResponses, "get all success");
        }

        public async Task<ApiResponse<ExamResponse>> getExamById(int Id)
        {
            var exam = await _dataContext.Exam
                .Include(exam => exam.Question)
                .ThenInclude(question => question.Answers)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (exam == null)
                return ApiResponse<ExamResponse>.Failure(404, "id exam not found");

            var examResponse = new ExamResponse
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                Duration = exam.Duration,
                Question = exam.Question.Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    ExamId = q.ExamId,
                    QuestionExam = q.QuestionExam,
                    Point = q.Point,
                    Answers = q.Answers.Select(a => new AnswerResponse
                    {
                        Id = a.Id,
                        QuestionId = a.QuestionId,
                        QuestionExam = a.Question.QuestionExam,
                        Answer1 = a.Answer1,
                        Answer2 = a.Answer2,
                        Answer3 = a.Answer3,
                        Answer4 = a.Answer4,
                        CorrectAnswer = a.CorrectAnswer
                    }).ToList()
                }).ToList()
            };

            return ApiResponse<ExamResponse>.Success(200, examResponse, "find success");
        }

        public async Task<ApiResponse<ExamResponse>> updateExam(int Id, ExamDTO examDTO)
        {
            var exam = await _dataContext.Exam.FindAsync(Id);
            if (exam == null) return ApiResponse<ExamResponse>.Failure(404, "id exam notfound");

            exam.Id = Id;
            exam.Title = examDTO.Title;
            exam.Description = examDTO.Description;
            exam.Duration = examDTO.Duration;
            exam.Update_at = DateTime.Now;

            var newExamDto = new ExamResponse
            {
                Title = exam.Title,
                Description = exam.Description,
                Duration = examDTO.Duration,
            };

            await _dataContext.SaveChangesAsync();

            return ApiResponse<ExamResponse>.Success(200, newExamDto, "update exam success");
        }

        public async Task<ApiResponse<object>> getExam(){
            var data = await _dataContext.Exam.ToListAsync();  
            return ApiResponse<object>.Success(200, data);
        }

        public async Task<ApiResponse<string>> deleteExam(int id){
            var exam = await _dataContext.Exam.FirstOrDefaultAsync(x => x.Id == id);
            if (exam == null) return ApiResponse<string>.Failure(404, "id not found");
            _dataContext.Exam.Remove(exam);
            await _dataContext.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "delete exam success");
        }
    }
}