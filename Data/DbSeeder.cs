using System.Linq;
using Microsoft.AspNet.Identity;
using ShopTheThao.Models;

namespace ShopTheThao.Data
{
    public static class DbSeeder
    {
        public static void Run(AppDbContext db)
        {
            var hasher = new PasswordHasher();

            if (!db.Users.Any())
            {
                db.Users.Add(new User
                {
                    FullName = "Quản trị viên",
                    Email = "admin@sportshop.vn",
                    PasswordHash = hasher.HashPassword("Admin@123"),
                    Role = "Admin",
                    Phone = "0900000001"
                });
                db.Users.Add(new User
                {
                    FullName = "Khách hàng mẫu",
                    Email = "khach@sportshop.vn",
                    PasswordHash = hasher.HashPassword("Khach@123"),
                    Role = "Customer",
                    Phone = "0900000002"
                });
            }

            if (!db.Categories.Any())
            {
                db.Categories.AddRange(new[] {
                    new Category { Name = "Bóng đá", Slug = "bong-da" },
                    new Category { Name = "Chạy bộ", Slug = "chay-bo" },
                    new Category { Name = "Gym", Slug = "gym" },
                    new Category { Name = "Cầu lông", Slug = "cau-long" },
                    new Category { Name = "Phụ kiện", Slug = "phu-kien" } });
                db.Brands.AddRange(new[] {
                    new Brand { Name = "Nike" }, new Brand { Name = "Adidas" },
                    new Brand { Name = "Puma" }, new Brand { Name = "Yonex" } });
                db.SaveChanges();

                var cat = db.Categories.ToList(); var br = db.Brands.ToList();
                var p1 = new Product { Name = "Giày đá bóng Nike Mercurial", Slug = "giay-nike-mercurial", Price = 2500000, SalePrice = 2190000, CategoryId = cat[0].Id, BrandId = br[0].Id, IsFeatured = true, SoldCount = 40, Description = "Giày đá bóng sân cỏ nhân tạo, nhẹ và bám sân tốt." };
                var p2 = new Product { Name = "Giày chạy bộ Adidas Ultraboost", Slug = "adidas-ultraboost", Price = 3200000, CategoryId = cat[1].Id, BrandId = br[1].Id, IsFeatured = true, SoldCount = 65, Description = "Đệm Boost êm ái cho mọi cự ly." };
                var p3 = new Product { Name = "Vợt cầu lông Yonex Astrox", Slug = "yonex-astrox", Price = 1800000, SalePrice = 1590000, CategoryId = cat[3].Id, BrandId = br[3].Id, SoldCount = 30, Description = "Vợt thiên công, khung carbon." };
                var p4 = new Product { Name = "Áo tập gym Puma Dry", Slug = "ao-gym-puma", Price = 450000, CategoryId = cat[2].Id, BrandId = br[2].Id, SoldCount = 90, Description = "Vải thoáng khí, nhanh khô." };
                db.Products.AddRange(new[] { p1, p2, p3, p4 });
                db.SaveChanges();

                foreach (var p in new[] { p1, p2, p4 })
                    foreach (var s in new[] { "39", "40", "41", "42" })
                        db.ProductVariants.Add(new ProductVariant { ProductId = p.Id, Size = s, Color = "Cam", Stock = 20 });
                db.ProductVariants.Add(new ProductVariant { ProductId = p3.Id, Size = "4U", Color = "Đen", Stock = 15 });

                db.Coupons.Add(new Coupon
                {
                    Code = "SPORT10",
                    DiscountPercent = 10,
                    MinOrder = 500000,
                    ExpireDate = System.DateTime.Now.AddMonths(3),
                    UsageLimit = 100
                });
                db.Banners.Add(new Banner { Title = "Trang bị cho mọi buổi tập", SortOrder = 1 });
            }
            db.SaveChanges();
        }
    }
}