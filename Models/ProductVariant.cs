using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [StringLength(20)] public string Size { get; set; }
        [StringLength(30)] public string Color { get; set; }
        public int Stock { get; set; }
        public virtual Product Product { get; set; }
    }

}