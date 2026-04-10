using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Model;

namespace NguyenDucHuy_2123110217_ASP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ===== DB SET =====
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_Variant> ProductVariants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Item> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Stock_Movement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // PRODUCT VARIANT
            // =========================
            modelBuilder.Entity<Product_Variant>()
                .HasIndex(x => x.SKU)
                .IsUnique();

            modelBuilder.Entity<Product_Variant>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product_Variant>()
                .Property(x => x.CostPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product_Variant>()
                .HasOne(pv => pv.Inventory)
                .WithOne(i => i.ProductVariant)
                .HasForeignKey<Inventory>(i => i.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product_Variant>()
                .HasMany(pv => pv.OrderItems)
                .WithOne(oi => oi.ProductVariant)
                .HasForeignKey(oi => oi.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CATEGORY - PRODUCT
            // =========================
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // CUSTOMER - ORDER
            // =========================
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // USER - ORDER
            // =========================
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // ORDER - ORDER_ITEM
            // =========================
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(x => x.FinalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order_Item>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);

            // =========================
            // ORDER - PAYMENT
            // =========================
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // =========================
            // INVENTORY - STOCK_MOVEMENT
            // =========================
            modelBuilder.Entity<Inventory>()
                .HasMany(i => i.StockMovements)
                .WithOne(sm => sm.Inventory)
                .HasForeignKey(sm => sm.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Stock_Movement>()
                .Property(sm => sm.Quantity)
                .IsRequired();
        }
    }
}