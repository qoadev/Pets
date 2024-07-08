using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication3.Data;
using WebApplication3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using Npgsql;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PetContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, PetContext context, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User userLogin)
        {
            try
            {
                _logger.LogInformation("Login attempt: {Name}", userLogin.Name);
                var query = "SELECT * FROM \"User\" WHERE \"Name\" = @name AND \"Password\" = @password;";
                var db = new Db();
                DataTable result = db.ExecuteQuery(query, new NpgsqlParameter("@name", userLogin.Name), new NpgsqlParameter("@password", userLogin.Password));
                
                if (result.Rows.Count > 0)
                {
                    var firstRow = result.Rows[0];
                    var name = firstRow["Name"].ToString();
                    var password = firstRow["Password"].ToString();
                    _logger.LogInformation("User found: {Name}", name);
                    
                    User user = new User
                    {
                        Name = name,
                        Password = password,
                    };
                    
                    var token = GenerateJwtToken(user);
                    _logger.LogInformation("Токен сгенерирован успешно.");
                    return Ok(new { token });
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user: {Name}", userLogin.Name);
                    return Unauthorized(new { Message = "Login or password error" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе Login.");
                return StatusCode(500, "Произошла ошибка на сервере.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Name)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:Expires"])),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании JWT токена.");
                throw;
            }
        }
    }
}
