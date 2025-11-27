using CB_WebApiTask.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CB_WebApiTask.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<OtpSession> OtpSessions => Set<OtpSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(x => x.IcNumber);
                e.HasIndex(x => x.MobileNumber).IsUnique();
                e.HasIndex(x => x.EmailAddress).IsUnique(false);
                e.Property(x => x.IcNumber).HasMaxLength(50);
                e.Property(x => x.MobileNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<OtpSession>(e =>
            {
                e.HasIndex(x => new { x.IcNumber, x.Purpose, x.IsVerified });
                e.HasIndex(x => new { x.MobileNumber, x.Purpose, x.IsVerified });
                e.Property(x => x.DeliveryMethod).HasMaxLength(10);
            });
        }

    }
}
