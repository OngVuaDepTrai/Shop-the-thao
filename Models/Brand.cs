using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; }
        public string LogoUrl { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }

}