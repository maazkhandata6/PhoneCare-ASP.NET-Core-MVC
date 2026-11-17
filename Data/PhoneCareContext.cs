using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;

namespace PhoneCare.Data;

public class PhoneCareContext : DbContext
{
    public PhoneCareContext(DbContextOptions<PhoneCareContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<RepairRequest> RepairRequests => Set<RepairRequest>();
    public DbSet<RepairService> RepairServices => Set<RepairService>();
    public DbSet<PhoneBrand> PhoneBrands => Set<PhoneBrand>();
    public DbSet<PhoneModel> PhoneModels => Set<PhoneModel>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<RepairRequest>().Property(x => x.EstimatedCost).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Payment>().HasOne(x => x.RepairRequest).WithMany().HasForeignKey(x => x.RepairRequestId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RepairService>().Property(x => x.BasePrice).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<RepairRequest>()
            .HasOne(x => x.Customer).WithMany(x => x.RepairRequests)
            .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RepairRequest>()
            .HasOne(x => x.RepairService).WithMany()
            .HasForeignKey(x => x.RepairServiceId).OnDelete(DeleteBehavior.Restrict);
    }
}
