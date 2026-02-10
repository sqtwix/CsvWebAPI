using CsvApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CsvApi.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Values> Values => Set<Values>();
        public DbSet<Results> Results => Set<Results>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
