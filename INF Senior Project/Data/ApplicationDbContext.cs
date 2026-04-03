using INF_Senior_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace INF_Senior_Project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: configure relationships explicitly
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Pharmacist)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.PharmacistId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
