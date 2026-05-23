using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // YARP Gateway motorunu yüklüyoruz
            builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            // KAPICIYA BÝLET KONTROLÜ ÖÐRETÝLÝYOR (JWT Doðrulama)
            var key = Encoding.ASCII.GetBytes("EduDeskSuperSecretKey1234567890987654321"); // Identity ile ayný þifre!
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false, // Test ortamý olduðu için doðrulamalarý hafiflettik
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Güvenlik sistemini aktif ediyoruz
            app.UseAuthentication();
            app.UseAuthorization();

            // Gelen istekleri ilgili odalara yönlendir
            app.MapReverseProxy();

            app.Run();
        }
    }
}