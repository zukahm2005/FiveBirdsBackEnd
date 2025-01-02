using five_birds_be.Data;
using five_birds_be.DTO.Request;
using five_birds_be.DTO.Response;
using five_birds_be.Models;
using five_birds_be.Response;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class UserExamService
    {

        public DataContext _dataContext;
        public UserExamService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<UserExamResponse>> postUserExam(UserExamRequest userExamRequest)
        {
            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.UserId == userExamRequest.UserId);
            if (user == null) return ApiResponse<UserExamResponse>.Failure(404, "userId not found");
            var exam = await _dataContext.Exam.FirstOrDefaultAsync(u => u.Id == userExamRequest.ExamId);
            if (exam == null) return ApiResponse<UserExamResponse>.Failure(404, "examId not found");

            var newUserExam = new User_Eaxam
            {
                UserId = userExamRequest.UserId,
                User = user,
                ExamId = userExamRequest.ExamId,
                Exam = exam,
                TestStatus = userExamRequest.TestStatus,
                ExamTime = userExamRequest.ExamTime,
                ExamDate = userExamRequest.ExamDate,
            };
            await _dataContext.User_Exams.AddAsync(newUserExam);
            await _dataContext.SaveChangesAsync();

            var newUserExamResponse = new UserExamResponse
            {
                Id = newUserExam.Id,
                UserId = newUserExam.UserId,
                ExamId = newUserExam.ExamId,
                TestStatus = newUserExam.TestStatus,
                ExamTime = newUserExam.ExamTime,
                ExamDate = newUserExam.ExamDate,
            };
            return ApiResponse<UserExamResponse>.Success(200, newUserExamResponse);
        }
        public async Task<ApiResponse<object>> GetUserExamAll(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            var data = await _dataContext.User_Exams
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(ue => new
            {
                ue.Id,
                User = new
                {
                    ue.UserId,
                    ue.User.UserName,
                    ue.User.Email
                },
                Exam = new
                {
                    ue.ExamId,
                    ue.Exam.Title,
                    ue.Exam.Description,
                    ue.Exam.Duration
                },
                ue.TestStatus,
                ue.ExamTime,
                ue.ExamDate
            })
            .ToListAsync();

            return ApiResponse<object>.Success(200, data);
        }

        public async Task<ApiResponse<object>> GetUserExamById(int id)
        {
            var userExam = await _dataContext.User_Exams
                .Where(ue => ue.UserId == id)
                .Select(ue => new
                {
                    ue.Id,
                    User = new
                    {
                        ue.UserId,
                        ue.User.UserName,
                        ue.User.Email
                    },
                    Exam = new
                    {
                        ue.ExamId,
                        ue.Exam.Title,
                        ue.Exam.Description,
                        ue.Exam.Duration
                    },
                    ue.TestStatus,
                    ue.ExamTime,
                    ue.ExamDate
                })
                .ToListAsync();

            if (userExam == null)
            {
                return ApiResponse<object>.Failure(404, "User exam not found.");
            }

            return ApiResponse<object>.Success(200, userExam);
        }


        public async Task<ApiResponse<UserExamResponse>> updateUserExam(int id, UserExamRequest userExamRequest)
        {
            var userExam = await _dataContext.User_Exams.FirstOrDefaultAsync(u => u.Id == id);
            if (userExam == null) return ApiResponse<UserExamResponse>.Failure(404, "id user exam not found");
            userExam.UserId = userExamRequest.UserId;
            userExam.ExamId = userExamRequest.ExamId;
            userExam.TestStatus = userExamRequest.TestStatus;
            userExam.ExamTime = userExamRequest.ExamTime;
            userExam.ExamDate = userExamRequest.ExamDate;

            await _dataContext.SaveChangesAsync();

            var newUserExam = new UserExamResponse
            {
                Id = userExam.Id,
                UserId = userExam.UserId,
                ExamId = userExam.ExamId,
                TestStatus = userExam.TestStatus,
                ExamTime = userExam.ExamTime,
                ExamDate = userExam.ExamDate
            };
            return ApiResponse<UserExamResponse>.Success(200, newUserExam);
        }
        public async Task<ApiResponse<string>> deleteUserExam(int id)
        {
            var userExam = await _dataContext.User_Exams.FirstOrDefaultAsync(u => u.Id == id);
            if (userExam == null) return ApiResponse<string>.Failure(404, "id user exam not found");
            _dataContext.Remove(userExam);
            await _dataContext.SaveChangesAsync();
            return ApiResponse<string>.Success(200, "delete user exam success");
        }
    }
}