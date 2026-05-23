using System;

namespace SupportService.Models
{
    public class Ticket
    {
        public Guid Id { get; set; } // Biletin benzersiz ID'si
        public string Title { get; set; } // Þikayet Baþlýðý (Örn: Videolar açýlmýyor)
        public string Description { get; set; } // Þikayet Detayý
        public string Status { get; set; } // Durum: Open, InProgress, Resolved

        // MÜLAKATIN EN KRÝTÝK ALANI: Bu bilet hangi kuruma ait?
        public Guid TenantId { get; set; }
    }
}