using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using five_birds_be.Models;
using Microsoft.IdentityModel.Tokens;

namespace five_birds_be.Jwt
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;


        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateJwtToken(Candidate candidate)
        {
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new JwtConfigurationException("JWT Key is missing or empty in configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Role", candidate.Role.ToString()),
                new Claim("CandidateId", candidate.CandidateId.ToString()),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? GetCandidateIdFromHttpContext()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var candidateIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "CandidateId");
            if (candidateIdClaim != null && int.TryParse(candidateIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }



    public class JwtConfigurationException : Exception
    {
        public JwtConfigurationException(string message) : base(message) { }
    }
}
