using System;
using System.Collections.Generic;
using ShopTheThao.Models;

namespace ShopTheThao.ViewModels
{
    public class ProductListViewModel
    {
        // Tham số tìm kiếm & lọc
        public string q { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Size { get; set; }
        public bool? OnSale { get; set; }
        public string SortBy { get; set; } = "newest";

        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 9));

        // Danh sách kết quả
        public List<Product> Products { get; set; } = new List<Product>();

        // Dữ liệu bộ lọc trên Sidebar
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<string> AvailableSizes { get; set; } = new List<string>();
        public Dictionary<int, int> CategoryCounts { get; set; } = new Dictionary<int, int>();

        // Nhãn đang chọn
        public string SelectedCategoryName { get; set; }
        public string SelectedBrandName { get; set; }

        public bool HasActiveFilters =>
            !string.IsNullOrWhiteSpace(q) ||
            (CategoryId.HasValue && CategoryId.Value > 0) ||
            (BrandId.HasValue && BrandId.Value > 0) ||
            MinPrice.HasValue ||
            MaxPrice.HasValue ||
            !string.IsNullOrWhiteSpace(Size) ||
            (OnSale.HasValue && OnSale.Value);
    }

    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public List<string> Sizes { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
        public int TotalStock { get; set; }

        // Đánh giá sản phẩm
        public List<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; } = 5.0;
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingCounts { get; set; } = new Dictionary<int, int>();

        // Sản phẩm liên quan & yêu thích
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
        public bool IsInWishlist { get; set; }
    }
}

