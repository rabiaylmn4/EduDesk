using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Data
{
    // DbContext, Entity Framework Core'un veritabaný yönetim merkezidir.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Veritabanýnda oluþacak tablolarýmýzý (DbSet) buraya kaydediyoruz
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ýleride buraya kiracý izolasyonu (Multi-Tenancy) için 
            // otomatik filtreleme kurallarý yazacaðýz.
        }
    }
}