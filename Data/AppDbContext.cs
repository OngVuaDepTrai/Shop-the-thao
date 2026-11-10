using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ShopTheThao.Models;

namespace ShopTheThao.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("SportShopDb") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Banner> Banners { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Conventions.Remove<OneToManyCascadeDeleteConvention>(); // tránh lỗi cascade nhiều đường
            mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
            mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 0);
            mb.Entity<Product>().Property(p => p.SalePrice).HasPrecision(18, 0);
            mb.Entity<Order>().Property(o => o.Total).HasPrecision(18, 0);
            mb.Entity<Order>().Property(o => o.DiscountAmount).HasPrecision(18, 0);
            mb.Entity<OrderItem>().Property(o => o.UnitPrice).HasPrecision(18, 0);
            mb.Entity<Coupon>().Property(c => c.MinOrder).HasPrecision(18, 0);
            mb.Entity<Coupon>().Property(c => c.MaxDiscount).HasPrecision(18, 0);
        }
    }
}