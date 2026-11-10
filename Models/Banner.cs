using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopTheThao.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}