using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandsController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();

        public ActionResult Index()
        {
            ViewBag.Title = "Thương hiệu";

            var brands = db.Brands
                           .Include(b => b.Products)
                           .OrderBy(b => b.Name)
                           .ToList();

            return View(brands);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "Thêm thương hiệu";

            return View(new BrandFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(BrandFormViewModel model)
        {
            ViewBag.Title = "Thêm thương hiệu";

            Normalize(model);
            Validate(model, null);

            if (!ModelState.IsValid) return View(model);

            db.Brands.Add(new Brand
            {
                Name = model.Name,
                LogoUrl = string.IsNullOrWhiteSpace(model.LogoUrl) ? null : model.LogoUrl
            });
            db.SaveChanges();

            TempData["Success"] = "Đã thêm thương hiệu \"" + model.Name + "\".";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var brand = db.Brands.Find(id);
            if (brand == null) return HttpNotFound();

            ViewBag.Title = "Sửa thương hiệu";

            return View(new BrandFormViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                LogoUrl = brand.LogoUrl
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(BrandFormViewModel model)
        {
            ViewBag.Title = "Sửa thương hiệu";

            var brand = db.Brands.Find(model.Id);
            if (brand == null) return HttpNotFound();

            Normalize(model);
            Validate(model, model.Id);

            if (!ModelState.IsValid) return View(model);

            brand.Name = model.Name;
            brand.LogoUrl = string.IsNullOrWhiteSpace(model.LogoUrl) ? null : model.LogoUrl;
            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật thương hiệu \"" + brand.Name + "\".";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var brand = db.Brands
                          .Include(b => b.Products)
                          .FirstOrDefault(b => b.Id == id);

            if (brand == null)
            {
                TempData["Error"] = "Thương hiệu không tồn tại.";
                return RedirectToAction("Index");
            }

            if (brand.Products.Any())
            {
                TempData["Error"] = "Không thể xóa thương hiệu \"" + brand.Name +
                                    "\" vì đang có " + brand.Products.Count + " sản phẩm.";
                return RedirectToAction("Index");
            }

            db.Brands.Remove(brand);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa thương hiệu \"" + brand.Name + "\".";
            return RedirectToAction("Index");
        }

        private void Normalize(BrandFormViewModel model)
        {
            model.Name = model.Name == null ? null : model.Name.Trim();
            model.LogoUrl = model.LogoUrl == null ? null : model.LogoUrl.Trim();
        }

        private void Validate(BrandFormViewModel model, int? id)
        {
            if (!string.IsNullOrEmpty(model.Name) &&
                db.Brands.Any(b => b.Name == model.Name && (!id.HasValue || b.Id != id.Value)))
            {
                ModelState.AddModelError("Name", "Tên thương hiệu đã tồn tại");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
