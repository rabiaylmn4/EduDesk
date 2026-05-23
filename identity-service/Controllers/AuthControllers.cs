using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.Models;
using IdentityService.Services;
using System;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthControllers : ControllerBase // Sýnýf adý dosya adýnla eþitlendi
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthControllers(AppDbContext context) // Constructor adý eþitlendi
        {
            _context = context;
            _tokenService = new TokenService();
        }

        // 1. KULLANICI KAYIT (REGISTER) ENDPOINT'I
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User userDto)
        {
            // Email sistemde var mý kontrolü
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                return BadRequest("Bu e-posta adresi zaten kullanýmda.");

            // Þifreyi BCrypt ile güvenli bir þekilde hash'liyoruz (Düz metin saklanamaz!)
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                Role = userDto.Role,
                TenantId = userDto.TenantId // Kurum ID'si
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanýcý baþarýyla kaydedildi." });
        }

        // 2. KULLANICI GÝRÝÞÝ (LOGIN) ENDPOINT'I
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
        {
            // Kullanýcýyý e-posta adresiyle veritabanýnda ara
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return Unauthorized("Geçersiz e-posta veya þifre.");

            // Girilen þifreyi BCrypt ile doðrula
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid) return Unauthorized("Geçersiz e-posta veya þifre.");

            // Her þey doðruysa kullanýcýya JWT dijital anahtarýný üret ve dön
            var token = _tokenService.GenerateToken(user);

            return Ok(new { Token = token, Message = "Giriþ Baþarýlý!" });
        }
    }
}