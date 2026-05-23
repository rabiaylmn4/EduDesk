using System;

namespace IdentityService.Models
{
    public class User
    {
        public Guid Id { get; set; } // Kullanýcýnýn benzersiz ID'si
        public string Name { get; set; } // Adý Soyadý
        public string Email { get; set; } // Giriþ e-posta adresi
        public string PasswordHash { get; set; } // Þifrelenmiþ þifre
        public string Role { get; set; } // Rolü: Admin, Instructor, Student, Support

        // Bu kullanýcýnýn hangi kuruma ait olduðunu belirten bað
        public Guid TenantId { get; set; }
    }
}