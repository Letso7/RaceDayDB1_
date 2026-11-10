using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayDB1.Data;
using RaceDayDB1.Models;

namespace RaceDayDB1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDBContext _context;
        public AuthController(RaceDayDBContext context) { _context = context; }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // BCrypt 
                Role = dto.Role == "Organiser" ? "Organiser" : "Participant",
                FullName = dto.FullName
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return Unauthorized("Invalid login");

            // BCrypt Verify 
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid) return Unauthorized("Invalid login");

            // Session Management 
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("Username", user.Username);

            return Ok(new { message = "Logged in", user.Role, user.UserId });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out");
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized("Not logged in");
            return Ok(new { UserId = userId, Role = HttpContext.Session.GetString("UserRole") });
        }
    }
}