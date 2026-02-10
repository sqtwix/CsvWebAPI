using CsvApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CsvApi.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Value> Values => Set<Value>();
        public DbSet<Result> Results => Set<Result>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
