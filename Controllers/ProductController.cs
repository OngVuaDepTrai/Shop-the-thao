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
        public ActionResult Index(ProductListViewModel filter)
        {
            if (filter == null) filter = new ProductListViewModel();

            // Backward compatibility for query string ?category=1
            if (!filter.CategoryId.HasValue && !string.IsNullOrEmpty(Request.QueryString["category"]))
            {
                int catId;
                if (int.TryParse(Request.QueryString["category"], out catId))
                {
                    filter.CategoryId = catId;
                }
            }

            var query = db.Products.Include(p => p.Category).Include(p => p.Brand).Where(p => p.IsActive);

            // 1. Search keyword (Name or Code)
            if (!string.IsNullOrWhiteSpace(filter.q))
            {
                var term = filter.q.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) || (p.Code != null && p.Code.ToLower().Contains(term)));
            }

            // 2. Filter by Category
            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            // 3. Filter by Brand
            if (filter.BrandId.HasValue && filter.BrandId.Value > 0)
            {
                query = query.Where(p => p.BrandId == filter.BrandId.Value);
            }

            // 4. Filter by Price Range
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => (p.SalePrice ?? p.Price) <= filter.MaxPrice.Value);
            }

            // 5. Filter by Size
            if (!string.IsNullOrWhiteSpace(filter.Size))
            {
                query = query.Where(p => p.ProductVariants.Any(v => v.Size == filter.Size && v.Stock > 0));
            }

            // 6. Filter by OnSale
            if (filter.OnSale.HasValue && filter.OnSale.Value)
            {
                query = query.Where(p => p.SalePrice != null && p.SalePrice < p.Price);
            }

            // 7. Sort
            switch (filter.SortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.SalePrice ?? p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.SalePrice ?? p.Price);
                    break;
                case "best_seller":
                    query = query.OrderByDescending(p => p.SoldCount);
                    break;
                case "newest":
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            // 8. Pagination
            filter.TotalItems = query.Count();
            if (filter.Page < 1) filter.Page = 1;
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 12;
            filter.Products = query.Skip((filter.Page - 1) * pageSize).Take(pageSize).ToList();

            // Populate Filter options for the View
            filter.Categories = db.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();
            filter.Brands = db.Brands.OrderBy(b => b.Name).ToList();

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
        public ActionResult Details(int? id, string slug = null)
        {
            if (!id.HasValue) return RedirectToAction("Index");

            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductVariants)
                .Include(p => p.Reviews.Select(r => r.User))
                .FirstOrDefault(p => p.Id == id.Value && p.IsActive);

            if (product == null) return HttpNotFound();

            // Gallery photos using team helper
            var gallery = ProductImage.Gallery(product);

            // Related products (same category or brand)
            var related = db.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
                .OrderByDescending(p => p.SoldCount)
                .Take(4)
                .ToList();

            // Distinct Sizes & Colors
            var sizes = product.ProductVariants
                .Where(v => v.Stock > 0 && !string.IsNullOrEmpty(v.Size))
                .Select(v => v.Size)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            var colors = product.ProductVariants
                .Where(v => v.Stock > 0 && !string.IsNullOrEmpty(v.Color))
                .Select(v => v.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // Reviews
            var reviews = product.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
            double avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 5.0;

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                GalleryImages = gallery,
                RelatedProducts = related,
                Reviews = reviews,
                AverageRating = Math.Round(avgRating, 1),
                TotalReviews = reviews.Count,
                Sizes = sizes,
                Colors = colors
            };

            return View(vm);
        }

        // POST: Product/AddReview
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(int productId, int rating, string comment)
        {
            var product = db.Products.Find(productId);
            if (product == null) return HttpNotFound();

            var currentUserId = User.UserId();
            if (currentUserId <= 0) return RedirectToAction("Login", "Account");

            var review = new Review
            {
                ProductId = productId,
                UserId = currentUserId,
                Rating = Math.Max(1, Math.Min(5, rating)),
                Comment = comment?.Trim(),
                CreatedAt = DateTime.Now,
                IsApproved = true // Tự duyệt cho đồ án sinh viên để hiển thị ngay
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
