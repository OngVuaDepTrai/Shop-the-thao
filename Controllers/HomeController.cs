using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopTheThao.Data;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var active = db.Products.Include(p => p.Category).Where(p => p.IsActive);
            var vm = new HomeViewModel
            {
                Categories = db.Categories.Where(c => c.IsActive).ToList(),
                NewProducts = active.OrderByDescending(p => p.CreatedAt).Take(8).ToList(),
                BestSellers = active.OrderByDescending(p => p.SoldCount).Take(8).ToList(),
                SaleProducts = active.Where(p => p.SalePrice != null)
                                     .OrderByDescending(p => p.CreatedAt).Take(8).ToList()
            };
            return View(vm);
        }

        public ActionResult About() { return View(); }
        public ActionResult Contact() { return View(); }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}