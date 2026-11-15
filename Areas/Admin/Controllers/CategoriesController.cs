using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();

        public ActionResult Index()
        {
            ViewBag.Title = "Danh mục";

            var categories = db.Categories
                               .Include(c => c.Products)
                               .OrderBy(c => c.Name)
                               .ToList();

            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "Thêm danh mục";

            return View(new CategoryFormViewModel { IsActive = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(CategoryFormViewModel model)
        {
            ViewBag.Title = "Thêm danh mục";

            Normalize(model);
            Validate(model, null);

            if (!ModelState.IsValid) return View(model);

            db.Categories.Add(new Category
            {
                Name = model.Name,
                Slug = FinalSlug(model),
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl,
                IsActive = model.IsActive
            });
            db.SaveChanges();

            TempData["Success"] = "Đã thêm danh mục \"" + model.Name + "\".";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            ViewBag.Title = "Sửa danh mục";

            return View(new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ImageUrl = category.ImageUrl,
                IsActive = category.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryFormViewModel model)
        {
            ViewBag.Title = "Sửa danh mục";

            var category = db.Categories.Find(model.Id);
            if (category == null) return HttpNotFound();

            Normalize(model);
            Validate(model, model.Id);

            if (!ModelState.IsValid) return View(model);

            category.Name = model.Name;
            category.Slug = FinalSlug(model);
            category.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl;
            category.IsActive = model.IsActive;
            db.SaveChanges();

            TempData["Success"] = "Đã cập nhật danh mục \"" + category.Name + "\".";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = db.Categories
                             .Include(c => c.Products)
                             .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                TempData["Error"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }

            if (category.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục \"" + category.Name +
                                    "\" vì đang có " + category.Products.Count + " sản phẩm.";
                return RedirectToAction("Index");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            TempData["Success"] = "Đã xóa danh mục \"" + category.Name + "\".";
            return RedirectToAction("Index");
        }

        private void Normalize(CategoryFormViewModel model)
        {
            model.Name = model.Name == null ? null : model.Name.Trim();
            model.Slug = model.Slug == null ? null : model.Slug.Trim();
            model.ImageUrl = model.ImageUrl == null ? null : model.ImageUrl.Trim();
        }

        private void Validate(CategoryFormViewModel model, int? id)
        {
            if (!string.IsNullOrEmpty(model.Name) &&
                db.Categories.Any(c => c.Name == model.Name && (!id.HasValue || c.Id != id.Value)))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại");
            }

            var slug = string.IsNullOrEmpty(model.Slug) ? ToSlug(model.Name) : ToSlug(model.Slug);

            if (!string.IsNullOrEmpty(slug) &&
                db.Categories.Any(c => c.Slug == slug && (!id.HasValue || c.Id != id.Value)))
            {
                ModelState.AddModelError("Slug", "Slug đã được dùng cho danh mục khác");
            }
        }

        private static string FinalSlug(CategoryFormViewModel model)
        {
            var slug = string.IsNullOrEmpty(model.Slug) ? ToSlug(model.Name) : ToSlug(model.Slug);
            return string.IsNullOrEmpty(slug) ? null : slug;
        }

        private static string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            var normalized = input.Trim().ToLowerInvariant().Replace('đ', 'd');

            var builder = new StringBuilder();
            foreach (var c in normalized.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            var result = builder.ToString().Normalize(NormalizationForm.FormC);
            result = Regex.Replace(result, @"[^a-z0-9\s-]", "");
            result = Regex.Replace(result, @"\s+", "-");
            result = Regex.Replace(result, "-+", "-").Trim('-');

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
