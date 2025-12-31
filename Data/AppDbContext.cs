using Microsoft.EntityFrameworkCore;
using AviaPartsAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AviaPartsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Part> Parts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Part>()
              .HasIndex(p => p.SerialNumber)
              .IsUnique();

            modelBuilder.Entity<Part>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
        }
    }
}
