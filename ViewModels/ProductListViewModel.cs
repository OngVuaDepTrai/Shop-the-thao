using System;
using System.Collections.Generic;
using ShopTheThao.Models;

namespace ShopTheThao.ViewModels
{
    public class ProductListViewModel
    {
        // Search & Filter parameters
        public string q { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Size { get; set; }
        public bool? OnSale { get; set; }
        public string SortBy { get; set; } = "newest";

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 12));

        // Result Data
        public List<Product> Products { get; set; } = new List<Product>();

        // Filter Options for View
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<string> AvailableSizes { get; set; } = new List<string>();

        // Selected metadata for display
        public string SelectedCategoryName { get; set; }
        public string SelectedBrandName { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; } = 5.0;
        public int TotalReviews { get; set; }
        public List<string> Sizes { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
    }
}

