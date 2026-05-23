using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportService.Data;
using SupportService.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SupportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly SupportDbContext _context;

        public TicketsController(SupportDbContext context)
        {
            _context = context;
        }

        // 1. SADECE GÝRÝÞ YAPAN KURUMUN BÝLETLERÝNÝ GETÝR (MULTI-TENANT SELECTION)
        [HttpGet]
        public async Task<IActionResult> GetMyTickets()
        {
            // MÜLAKAT ÞOVU: Kapýnýn (Gateway) bilet içinden okuyup bize fýsýldadýðý TenantId'yi yakalýyoruz
            var tenantIdHeader = Request.Headers["X-Tenant-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(tenantIdHeader))
                return Unauthorized("Kurum bilgisi doðrulanamadý, içeri sýzma engellendi.");

            var tenantId = Guid.Parse(tenantIdHeader);

            // VERÝ ÝZOLASYONU: Herkes sadece kendi kurumunun biletlerini görebilir!
            var tickets = await _context.Tickets
                .Where(t => t.TenantId == tenantId)
                .ToListAsync();

            return Ok(tickets);
        }

        // 2. YENÝ DESTEK BÝLETÝ OLUÞTUR
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticketDto)
        {
            var tenantIdHeader = Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantIdHeader)) return Unauthorized();

            var newTicket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                Status = "Open",
                TenantId = Guid.Parse(tenantIdHeader) // Otomatik olarak bilet açanýn kurumuna baðlanýyor
            };

            _context.Tickets.Add(newTicket);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Destek bileti baþarýyla oluþturuldu." });
        }
    }
}