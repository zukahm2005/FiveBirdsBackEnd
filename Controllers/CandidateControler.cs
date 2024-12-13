using System.IdentityModel.Tokens.Jwt;
using five_birds_be.Data;
using five_birds_be.Dto;
using five_birds_be.DTO.Request;
using five_birds_be.Models;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/candidates")]
    public class CandidateController : ControllerBase
    {
        private readonly CandidateService _candidateService;
        public CandidateController(CandidateService candidateService, DataContext datacontext)
        {
            _candidateService = candidateService;
        }

        [HttpGet("all/{pageNumber}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAllCandidate(int pageNumber)
        {
            var candidate = await _candidateService.GetCandidatesPaged(pageNumber);
            if (candidate == null || !candidate.Any())
                return NotFound(ApiResponse<List<Candidate>>.Failure(404, "No candidate found"));

            return Ok(ApiResponse<List<Candidate>>.Success(200, candidate, "Candidate retrieved successfully"));
        }


        [HttpPost("register")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> Register([FromBody] CandidateDTO candidate)
        {
            var postCandidate = await _candidateService.Register(candidate);

            if (postCandidate.ErrorCode == 400) return BadRequest(postCandidate);

            return Ok(postCandidate);
        }

        [HttpPut("update")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> UpdateCandidate([FromBody] CandidateDTO candidate)
        {
            var data = await _candidateService.UpdateCandidate(candidate);

            if (data.ErrorCode == 404) return NotFound(data);

            if (data.ErrorCode == 400) return BadRequest(data);

            return Ok(data);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CandidateLoginDTO candidateDTO)
        {
            var data = await _candidateService.Login(candidateDTO);

            if (data.ErrorCode == 400) return BadRequest(data);

            if (data.ErrorCode == 500) return StatusCode(500, data);
            ;

            return Ok(data);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Append("token", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1)
                });

                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error when logging out", error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CANDIDATE")]
        public async Task<IActionResult> GetCandidateById()
        {
            var data = await _candidateService.GetCandidateById();
            if (data.ErrorCode == 404) BadRequest(data);
            if (data.ErrorCode == 400) BadRequest(data);
            return Ok(data);
        }


        [HttpGet("checktoken")]
        public IActionResult CheckToken()
        {
            try
            {
                string? token = Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return Ok(new { isLoggedIn = false, message = "Not logged in." });
                }

                var handler = new JwtSecurityTokenHandler();
                try
                {
                    var claims = handler.ReadJwtToken(token).Claims;

                    var resultList = claims.Select(c => new
                    {
                        Type = c.Type,
                        Value = c.Value
                    }).ToList();

                    return Ok(new { isLoggedIn = true, claims = resultList, message = "Already logged in." });
                }
                catch
                {
                    return BadRequest(new { isLoggedIn = false, message = "Invalid token." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isLoggedIn = false, message = "Error checking token.", error = ex.Message });
            }
        }


        [HttpGet("checkrole")]
        public IActionResult CheckRole()
        {
            try
            {
                string? token = Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Token does not exist in cookie." });
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
                if (string.IsNullOrEmpty(roleClaim))
                {
                    return BadRequest(new { message = "Role does not exist in token." });
                }

                return Ok(new { role = roleClaim, message = "Get role successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error when checking the relay.", error = ex.Message });
            }
        }

    }
}