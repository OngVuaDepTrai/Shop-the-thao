using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class InventoryLog
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int Change { get; set; }
        public string Type { get; set; } // Import | Export
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ProductVariant ProductVariant { get; set; }
    }
}