using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual User User { get; set; }
        public virtual ICollection<CartItem> Items { get; set; }
    }
}