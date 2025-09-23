using Azure.Core;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserGroupManager.Api.DataTranferObjects;
using UserGroupManager.Domain.Entities;
using UserGroupManager.Infrastructure.Data;

namespace UserGroupManager.Api.Controllers
{

    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private  readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register(RegisterUserDTO request)
        {
            if(await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User with this email already Exist.");
            }

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AccountUpdated = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new {Message ="User registered successfully."});

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            var authToken = GenerateJwtToken(user);


            return Ok(new
            {
                message = "Login successful.",
                AuthToken = authToken,
                User = new
                {
                    user.FirstName,
                    user.Email
                }
            });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim (ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpGet("hash-password/{password}")]
        public IActionResult HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

            return Ok(new
            {
                originalPassword = password,
                bcryptHash = hashedPassword
            });
        }
    }
}
