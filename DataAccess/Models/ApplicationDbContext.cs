using Microsoft.EntityFrameworkCore;
using LibraryAPI.DataAccess.Models;

namespace LibraryAPI.DataAccess.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // Add DbSet for other tables here (e.g., Books, FinePayments, etc.)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure 'TotalFine' property with precision and scale
            modelBuilder.Entity<User>()
                .Property(u => u.TotalFine)
                .HasColumnType("decimal(10,2)")  // Specify decimal type with precision and scale
                .HasDefaultValue(0);  // Optional: Set a default value for TotalFine

            // Additional configurations for other entities, if needed
        }
    }
}
