using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace five_birds_be.Controllers
{

    [ApiController]
    [Route("api/v1/results")]

    public class ResultController : ControllerBase
    {
        public ResultService _resultService;
        private readonly ILogger<ResultController> _logger;
        public ResultController(ResultService resultService, ILogger<ResultController> logger)
        {
            _resultService = resultService;
            _logger = logger;
        }

        [HttpPost("add")]
        [Authorize(Roles = "ROLE_CANDIDATE")]
        public async Task<IActionResult> AddResults([FromBody] List<ResultDTO> results)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errorCode = 400,
                    message = "Invalid input data",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {

                _logger.LogInformation("Payload nhận được: {Payload}", JsonConvert.SerializeObject(results));


                var response = await _resultService.PostResults(results);

                if (response.ErrorCode == 200)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddResults: {ex.Message}");

                return StatusCode(500, ApiResponse<string>.Failure(500, "An unexpected error occurred."));
            }
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