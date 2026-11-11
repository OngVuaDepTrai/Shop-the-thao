using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();

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

        public ActionResult About() 
        { 
            return View(); 
        }

        // GET: Home/Contact
        public ActionResult Contact() 
        { 
            return View(); 
        }

        // POST: Home/Contact (Lưu vào bảng Contacts theo đúng phân công)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string name, string email, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ họ tên, email và nội dung tin nhắn!";
                return View();
            }

            var contact = new Contact
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Message = message.Trim(),
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            db.Contacts.Add(contact);
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Đội ngũ SportHub sẽ phản hồi trong vòng 24 giờ làm việc.";
            return RedirectToAction("Contact");
        }

        protected override void Dispose(bool disposing) 
        { 
            if (disposing) db.Dispose(); 
            base.Dispose(disposing); 
        }
    }
}
