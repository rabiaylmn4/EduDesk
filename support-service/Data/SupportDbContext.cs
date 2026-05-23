using Microsoft.EntityFrameworkCore;
using SupportService.Models;
using System.Collections.Generic;

namespace SupportService.Data
{
    public class SupportDbContext : DbContext
    {
        public SupportDbContext(DbContextOptions<SupportDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; } // Biletler tablomuz
    }
}