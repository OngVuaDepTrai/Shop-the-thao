using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(200)] public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string ImageUrl { get; set; }
        public string Images { get; set; }   // nhiều ảnh, ngăn cách bằng ';'
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool IsActive { get; set; } = true; public bool IsFeatured { get; set; }
        public int SoldCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual Category Category { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual ICollection<ProductVariant> Variants { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}