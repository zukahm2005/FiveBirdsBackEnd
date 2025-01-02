using five_birds_be.DTO.Request;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> add(UserExamRequest userExamRequest)
        {
            var data = await _userExamService.postUserExam(userExamRequest);
            return Ok(data);
        }
        [HttpGet("get/all")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> getALl(int pageNumber, int pageSize)
        {
            var data = await _userExamService.GetUserExamAll(pageNumber, pageSize);
            return Ok(data);
        }
        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<ActionResult> getById(int id)
        {
            var data = await _userExamService.GetUserExamById(id);
            if (data.ErrorCode == 404) return NotFound(data);
            return Ok(data);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> update(int id, [FromBody] UserExamRequest userExamRequest)
        {
            var data = await _userExamService.updateUserExam(id, userExamRequest);
            if (data.ErrorCode == 404) return NotFound(data);
            return Ok(data);
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> delete(int id)
        {
            var data = await _userExamService.deleteUserExam(id);
            if (data.ErrorCode == 404) return NotFound(data);
            return Ok(data);
        }
    }
}