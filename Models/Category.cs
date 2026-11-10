using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Product> Products { get; set; }
    }
}