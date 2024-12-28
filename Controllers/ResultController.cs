using five_birds_be.Dto;
using five_birds_be.Models;
using five_birds_be.Services;
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
        public async Task<ActionResult> add(ResultDTO result)
        {
            var data = await _resultService.PostResult(result);
            return Ok(data);

        }
        [HttpGet("get/all")]
        public async Task<ActionResult> getAll(int pageNumber, int pageSize)
        {
            var result = await _resultService.GetAll(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpGet("get/{id}")]
        public async Task<ActionResult> getAllById(int id)
        {
            var result = await _resultService.GetById(id);
            return Ok(result);
        }
    }
}