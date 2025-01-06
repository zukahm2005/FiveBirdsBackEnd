using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/results")]
    public class ResultController : ControllerBase
    {
        public ResultService _resultService;
        public ResultController(ResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ROLE_CANDIDATE")]
        public async Task<ActionResult> add(ResultDTO result)
        {
            var data = await _resultService.PostResult(result);
            if (data.ErrorCode == 400) return BadRequest(data);
            return Ok(data);

        }
        [HttpGet("get/all")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<ActionResult> getAll(int pageNumber, int pageSize)
        {
            var result = await _resultService.GetAll(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpGet("get/{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<ActionResult> getAllById(int id)
        {
            var result = await _resultService.GetById(id);
            if (result.ErrorCode == 404) return NotFound(result);
            return Ok(result);
        }
        [HttpPut("update/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> put(int id, [FromBody] ResultDTO result)
        {
            var data = await _resultService.updateResult(id, result);
            if (data.ErrorCode == 404) return NotFound(data);
            return Ok(data);
        }
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> delete(int id)
        {
            var result = await _resultService.deleteResult(id);
            if (result.ErrorCode == 404) return NotFound(result);
            return Ok(result);
        }
    }
}