using Microsoft.EntityFrameworkCore;
using CPMSDbFirst.Models;

namespace CPMSDbFirst.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
           => optionsBuilder.UseSqlServer("Server=GLENN-LAPTOP;Database=CPMS;Trusted_Connection=True;");

        public ApplicationDbContext()
        {
        }

        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Default> Defaults { get; set; } = null!;
        public DbSet<Paper> Papers { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Reviewer> Reviewers { get; set; } = null!;

    }
}
