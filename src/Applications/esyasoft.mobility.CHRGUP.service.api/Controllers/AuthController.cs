using esyasoft.mobility.CHRGUP.service.api.DTOs.Auth;
using esyasoft.mobility.CHRGUP.service.api.Infrastructure.Security;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace esyasoft.mobility.CHRGUP.service.api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        //private readonly NotificationService _notificationService;
        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identifier = request.Identifier;

            // ADMIN
            var admin = await _db.admins
                .FirstOrDefaultAsync(a =>
                    a.Username == identifier || a.Email == identifier);

            if (admin != null && PasswordHasher.Verify(request.Password, admin.Password))
                return Ok(BuildToken(admin.Id, admin.Email, "admin"));

            // MANAGER
            var manager = await _db.managers
                .FirstOrDefaultAsync(m =>
                    m.Username == identifier || m.Email == identifier);

            if (manager != null && PasswordHasher.Verify(request.Password, manager.Password))
                return Ok(BuildToken(manager.Id, manager.Email, "manager"));

            // SUPERVISOR
            var supervisor = await _db.supervisors
                .FirstOrDefaultAsync(s =>
                    s.Username == identifier || s.Email == identifier);

            if (supervisor != null && PasswordHasher.Verify(request.Password, supervisor.Password))
                return Ok(BuildToken(supervisor.Id, supervisor.Email, "supervisor"));

            // DRIVER (PRIMARY USER)
            var driver = await _db.drivers
                .FirstOrDefaultAsync(d => d.Email == identifier);

            if (driver != null && PasswordHasher.Verify(request.Password, driver.Password))
                return Ok(BuildToken(driver.Id, driver.Email, "driver"));

            return Unauthorized("Invalid credentials");
        }

        private object BuildToken(string userId, string email, string role)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256)
            );

            return new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role
            };
        }
    }
}
