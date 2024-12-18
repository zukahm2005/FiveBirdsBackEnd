using five_birds_be.Data;
using five_birds_be.Dto;
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

        public async Task<List<Exam>> getAllExam(int pageNumber)
        {
            if (pageNumber < 1) pageNumber = 1;

            return await _dataContext.Exam
            .Include(exam => exam.Question)
            .ThenInclude(question => question.Answers)
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ToListAsync();
        }

        public async Task<ApiResponse<Exam>> getExamById(int Id){
            var exam = await _dataContext.Exam
            .Include(exam => exam.Question)
            .ThenInclude(question => question.Answers)
            .FirstOrDefaultAsync(c => c.Id == Id);

            if (exam == null) return ApiResponse<Exam>.Failure(404, "id exam not found");

            return  ApiResponse<Exam>.Success(200, exam, "find success");
        }

        public async Task<ApiResponse<ExamDTO>> updateExam(int Id, ExamDTO examDTO){
            var exam = await _dataContext.Exam.FindAsync(Id);
            if (exam == null) return  ApiResponse<ExamDTO>.Failure(404, "id exam notfound");

            exam.Id = Id;
            exam.Title = examDTO.Title;
            exam.Description = examDTO.Description;
            exam.Duration = examDTO.Duration;

            await _dataContext.SaveChangesAsync();

            return  ApiResponse<ExamDTO>.Success(200, null, "update exam success");
        }
    }
}