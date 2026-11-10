using System.Collections.Generic;
using ShopTheThao.Models;

namespace ShopTheThao.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> NewProducts { get; set; }
        public List<Product> BestSellers { get; set; }
        public List<Product> SaleProducts { get; set; }
    }
}