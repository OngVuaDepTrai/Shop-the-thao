using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [StringLength(20)] public string Status { get; set; } = "Pending"; // Pending|Confirmed|Shipping|Completed|Cancelled
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; } // COD | Transfer
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }
    }

}