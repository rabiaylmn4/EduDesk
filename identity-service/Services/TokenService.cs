using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using IdentityService.Models;

namespace IdentityService.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Güvenlik anahtarý: 32 karakterden uzun kurumsal bir þifre
            var key = Encoding.ASCII.GetBytes("EduDeskSuperSecretKey1234567890987654321");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //  Kullanýcýnýn kimlik bilgileri (Claims) token içine gömülüyor
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("TenantId", user.TenantId.ToString()) // Multi-Tenant Ýzolasyon baðý
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token 2 saat geçerli olsun
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}