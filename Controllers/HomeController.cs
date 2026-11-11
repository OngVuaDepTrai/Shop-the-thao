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

        public ActionResult About() { return View(); }
        public ActionResult Contact() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(string Name, string Email, string Phone, string Subject, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ họ tên, email và nội dung tin nhắn.";
                return View();
            }

            var fullMsg = Message.Trim();
            if (!string.IsNullOrWhiteSpace(Phone) || !string.IsNullOrWhiteSpace(Subject))
            {
                var prefix = "";
                if (!string.IsNullOrWhiteSpace(Phone)) prefix += "[SĐT: " + Phone.Trim() + "] ";
                if (!string.IsNullOrWhiteSpace(Subject)) prefix += "[Chủ đề: " + Subject.Trim() + "] ";
                fullMsg = prefix + fullMsg;
            }

            db.Contacts.Add(new Contact
            {
                Name = Name.Trim(),
                Email = Email.Trim(),
                Message = fullMsg,
                CreatedAt = DateTime.Now,
                IsRead = false
            });
            db.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi đã ghi nhận tin nhắn và sẽ phản hồi sớm nhất.";
            return RedirectToAction("Contact");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}