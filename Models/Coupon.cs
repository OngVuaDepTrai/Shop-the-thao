using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required, StringLength(30)] public string Code { get; set; }
        public int DiscountPercent { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal MinOrder { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; } = true;
    }

}