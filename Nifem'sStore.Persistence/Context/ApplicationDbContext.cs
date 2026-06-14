using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NifemsStore.Domain.Entities;
using NifemsStores.Domain.Entities;

namespace NifemsStores.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Brand > Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<PurchaseReport> PurchaseReports { get; set; }
        public DbSet<CartItem> CartItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.ApplicationUser)
                .WithOne(au => au.Admin)
                .HasForeignKey<Admin>(a => a.ApplicationUserId);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();


            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithOne(au => au.Customer)
                .HasForeignKey<Customer>(c => c.ApplicationUserId);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}
