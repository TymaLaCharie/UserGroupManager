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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private  readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult>Register([FromBody] RegisterUserDTO request)
        {
            if(await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User with this email already Exist.");
            }

            //As I dont have an Admin as of yet am gonna make the first person to register on this APP an Admin
            var isFirstUser = !await _context.Users.AnyAsync();

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                AccountUpdated = DateTime.UtcNow,

                //if this newUser is the first user am gonna make the user an Administrator and Approve immediately
                IsAdmin = isFirstUser,
                Status = isFirstUser ? UserStatus.Active : UserStatus.Pending
            };

            if (isFirstUser)
            {
                var adminGroup = await _context.Groups.FirstOrDefaultAsync(g => g.Name == "Administrators");
                if (adminGroup != null)
                {
                    var userGroupLink = new UserGroup
                    {
                        User = newUser,
                        Group = adminGroup,
                        Status = RequestStatus.Approved
                    };
                    _context.UserGroups.Add(userGroupLink);
                }
            }

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var message = isFirstUser
             ? "Admin account created and approved."
             : "Registration successful. Please wait for admin approval.";

            return Ok(new 
            {
                Message = message 
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }
            if (user.Status != UserStatus.Active)
            {
                return Unauthorized("Your account is pending approval.");
            }

            var permissions = await _context.UserGroups
                .Where(ug => ug.UserId == user.Id && ug.Status == RequestStatus.Approved)
                .SelectMany(ug => ug.Group.Permissions) 
                .Select(p => p.Name)
                .Distinct()
                .ToListAsync();

            var authToken = GenerateJwtToken(user, permissions); 



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

        private string GenerateJwtToken(User user, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
