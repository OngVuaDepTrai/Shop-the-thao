using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}