using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopTheThao.Helpers;
using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();

        // GET: Product
        public ActionResult Index(ProductListViewModel filter, int? category = null, int? brand = null, string sort = null, bool? sale = null)
        {
            if (filter == null) filter = new ProductListViewModel();

            // Tương thích với cả ?category=1 (từ Trang chủ của Tài) và ?CategoryId=1
            if (!filter.CategoryId.HasValue && category.HasValue)
            {
                filter.CategoryId = category.Value;
            }
            if (!filter.BrandId.HasValue && brand.HasValue)
            {
                filter.BrandId = brand.Value;
            }
            if (string.IsNullOrEmpty(Request.QueryString["SortBy"]) && !string.IsNullOrWhiteSpace(sort))
            {
                filter.SortBy = sort;
            }
            if (!filter.OnSale.HasValue && sale.HasValue)
            {
                filter.OnSale = sale.Value;
            }

            var query = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive);

            // 1. Tìm kiếm theo từ khóa (Tên SP, Mã SP, Mô tả, Danh mục, Thương hiệu)
            if (!string.IsNullOrWhiteSpace(filter.q))
            {
                var term = filter.q.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    (p.Code != null && p.Code.ToLower().Contains(term)) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(term)) ||
                    (p.Brand != null && p.Brand.Name.ToLower().Contains(term)));
            }

            // 2. Lọc theo Danh mục
            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            // 3. Lọc theo Thương hiệu
            if (filter.BrandId.HasValue && filter.BrandId.Value > 0)
            {
                query = query.Where(p => p.BrandId == filter.BrandId.Value);
            }

            // 4. Lọc theo Khoảng giá (tính trên giá thực bán SalePrice ?? Price)
            if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) <= filter.MaxPrice.Value);
            }

            // 5. Lọc theo Kích cỡ (Size còn tồn kho)
            if (!string.IsNullOrWhiteSpace(filter.Size))
            {
                var sizeTrimmed = filter.Size.Trim();
                query = query.Where(p => p.ProductVariants.Any(v => v.Size == sizeTrimmed && v.Stock > 0));
            }

            // 6. Lọc sản phẩm đang khuyến mãi
            if (filter.OnSale.HasValue && filter.OnSale.Value)
            {
                query = query.Where(p => p.SalePrice != null && p.SalePrice < p.Price);
            }

            // 7. Sắp xếp
            switch (filter.SortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.SalePrice ?? p.Price).ThenByDescending(p => p.Id);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.SalePrice ?? p.Price).ThenByDescending(p => p.Id);
                    break;
                case "best_seller":
                case "bestseller":
                    query = query.OrderByDescending(p => p.SoldCount).ThenByDescending(p => p.Id);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "newest":
                default:
                    filter.SortBy = "newest";
                    query = query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id);
                    break;
            }

            // 8. Phân trang
            filter.TotalItems = query.Count();
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 9;
            filter.PageSize = pageSize;
            var totalPages = filter.TotalPages > 0 ? filter.TotalPages : 1;
            if (filter.Page < 1) filter.Page = 1;
            if (filter.Page > totalPages) filter.Page = totalPages;

            filter.Products = query
                .Skip((filter.Page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Dữ liệu cho Sidebar Filter
            filter.Categories = db.Categories.Where(c => c.IsActive).OrderBy(c => c.Id).ToList();
            filter.Brands = db.Brands.OrderBy(b => b.Name).ToList();

            filter.CategoryCounts = db.Products
                .Where(p => p.IsActive)
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionary(x => x.CategoryId, x => x.Count);

            filter.AvailableSizes = db.ProductVariants
                .Where(v => v.Product.IsActive && v.Stock > 0 && v.Size != null && v.Size != "")
                .Select(v => v.Size)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            if (filter.CategoryId.HasValue)
            {
                var cat = filter.Categories.FirstOrDefault(c => c.Id == filter.CategoryId.Value);
                if (cat != null) filter.SelectedCategoryName = cat.Name;
            }

            if (filter.BrandId.HasValue)
            {
                var br = filter.Brands.FirstOrDefault(b => b.Id == filter.BrandId.Value);
                if (br != null) filter.SelectedBrandName = br.Name;
            }

            return View(filter);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductVariants)
                .Include(p => p.Reviews.Select(r => r.User))
                .FirstOrDefault(p => p.Id == id.Value && p.IsActive);

            if (product == null) return HttpNotFound();

            // Thư viện ảnh lấy bằng ProductImage.Gallery(product) theo quy ước nhóm
            var gallery = ProductImage.Gallery(product);

            var variants = product.ProductVariants.OrderBy(v => v.Id).ToList();
            var totalStock = variants.Sum(v => v.Stock);

            var sizes = variants
                .Where(v => !string.IsNullOrWhiteSpace(v.Size))
                .Select(v => v.Size)
                .Distinct()
                .ToList();

            var colors = variants
                .Where(v => !string.IsNullOrWhiteSpace(v.Color))
                .Select(v => v.Color)
                .Distinct()
                .ToList();

            // Đánh giá đã duyệt
            var approvedReviews = product.Reviews
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            double avgRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : 5.0;

            var ratingCounts = new Dictionary<int, int>();
            for (int star = 5; star >= 1; star--)
            {
                ratingCounts[star] = approvedReviews.Count(r => r.Rating == star);
            }

            // Sản phẩm liên quan cùng danh mục
            var related = db.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.Id != product.Id)
                .OrderByDescending(p => p.SoldCount)
                .Take(4)
                .ToList();

            if (related.Count < 4)
            {
                var existingIds = related.Select(r => r.Id).Concat(new[] { product.Id }).ToList();
                var fallback = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive && !existingIds.Contains(p.Id))
                    .OrderByDescending(p => p.SoldCount)
                    .Take(4 - related.Count)
                    .ToList();
                related.AddRange(fallback);
            }

            bool inWishlist = false;
            if (User.Identity.IsAuthenticated)
            {
                var uid = User.UserId();
                inWishlist = db.Wishlists.Any(w => w.UserId == uid && w.ProductId == product.Id);
            }

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                GalleryImages = gallery,
                Variants = variants,
                Sizes = sizes,
                Colors = colors,
                TotalStock = totalStock,
                Reviews = approvedReviews,
                AverageRating = Math.Round(avgRating, 1),
                TotalReviews = approvedReviews.Count,
                RatingCounts = ratingCounts,
                RelatedProducts = related,
                IsInWishlist = inWishlist
            };

            return View(vm);
        }

        // POST: Product/AddReview
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int rating = 5, string comment = "")
        {
            var product = db.Products.Find(productId);
            if (product == null) return HttpNotFound();

            var userId = User.UserId();
            if (userId <= 0) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["Error"] = "Vui lòng nhập nội dung nhận xét trước khi gửi đánh giá.";
                return RedirectToAction("Details", new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = Math.Max(1, Math.Min(5, rating)),
                Comment = comment.Trim(),
                CreatedAt = DateTime.Now,
                IsApproved = true
            };

            db.Reviews.Add(review);
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã gửi đánh giá sản phẩm!";
            return RedirectToAction("Details", new { id = productId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

