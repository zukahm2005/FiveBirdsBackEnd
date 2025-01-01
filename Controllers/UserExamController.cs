using five_birds_be.DTO.Request;
using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/user-exam")]
    public class UserExamController : ControllerBase
    {
        public UserExamService _userExamService { get; set; }
        public UserExamController(UserExamService userExamService)
        {
            _userExamService = userExamService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> add(UserExamRequest userExamRequest)
        {
            var data = await _userExamService.postUserExam(userExamRequest);
            return Ok(data);
        }
        [HttpGet("get/all")]
        public async Task<ActionResult> getALl(int pageNumber, int pageSize){
            var data = await _userExamService.GetUserExamAll(pageNumber, pageSize);
            return Ok(data);
        }
    }
}