using System;

namespace IdentityService.Models
{
    public class Tenant
    {
        public Guid Id { get; set; } // Her kurumun benzersiz ID'si
        public string Name { get; set; } // Kurum Adý (Örn: Kýrýkkale Üniversitesi)
        public string Plan { get; set; } // Paket Türü (Basic, Pro, Enterprise)
    }
}