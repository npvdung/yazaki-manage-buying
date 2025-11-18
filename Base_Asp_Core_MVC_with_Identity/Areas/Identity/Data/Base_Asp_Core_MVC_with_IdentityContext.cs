using Base_Asp_Core_MVC_with_Identity.Areas.Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MangagerBuyProduct.Models;

namespace Base_Asp_Core_MVC_with_Identity.Data;

public class Base_Asp_Core_MVC_with_IdentityContext : IdentityDbContext<UserSystemIdentity>
{
    public Base_Asp_Core_MVC_with_IdentityContext(DbContextOptions<Base_Asp_Core_MVC_with_IdentityContext> options)
        : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Import_Product> ImportsProduct { get; set; }
    public DbSet<Import_Product_Details> ImportProductDetails { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Invoice_Details> Invoice_Details { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Return_Product> ReturnProducts { get; set; }
    public DbSet<Return_Product_Details> Return_Product_Details { get; set; }
    public DbSet<Inventory> inventory { get; set; }
    public DbSet<Supplier> suppliers { get; set; }
    public DbSet<BillPayment> BillPayment { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<IngredientsGroup> ingredientsGroups { get; set; }
    public DbSet<ItemReceipt> ItemReceipt { get; set; }
    public DbSet<PurchaseContract> purchaseContracts { get; set; }
    public DbSet<PurchaseContractDetails> purchaseContractDetails { get; set; }
    public DbSet<PurchaseOrder> purchaseOrders { get; set; }
    public DbSet<PurchaseOrderDetails> purchaseOrderDetails { get; set; }
    public DbSet<Quota> quota { get; set; }
    public DbSet<ReturnAuthorization> returnAuthorizations { get; set; }
    public DbSet<ShipmentRequest> shipmentRequests { get; set; }
    public DbSet<Vendor> vendors { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }

    private class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<UserSystemIdentity>
    {
        public void Configure(EntityTypeBuilder<UserSystemIdentity> builder)
        {
            builder.Property(u => u.FirstName).HasMaxLength(150);
            builder.Property(u => u.LastName).HasMaxLength(150);
            builder.Property(u =>u.Address).HasMaxLength(128);
            builder.Property(u =>u.Department).HasMaxLength(128);
            builder.Property(u =>u.Code).HasMaxLength(128);
            builder.Property(u =>u.BirthDate).HasDefaultValue(DateTime.Now);
            builder.Property(u =>u.Status).HasDefaultValue(0);
        }
    }

}
