using CompanyAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace CompanyAPI.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Isin)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
