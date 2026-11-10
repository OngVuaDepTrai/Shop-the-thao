using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using ShopTheThao.Helpers;
using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();

        public ActionResult Index()
        {
            var u = db.Users.Find(User.UserId());
            if (u == null) return RedirectToAction("Login", "Account");
            return View(new ProfileViewModel { Email = u.Email, FullName = u.FullName, Phone = u.Phone, Address = u.Address, AvatarUrl = u.AvatarUrl });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateInfo(ProfileViewModel m)
        {
            var u = db.Users.Find(User.UserId());
            if (!ModelState.IsValid) { m.Email = u.Email; m.AvatarUrl = u.AvatarUrl; return View("Index", m); }
            u.FullName = m.FullName.Trim(); u.Phone = m.Phone; u.Address = m.Address;
            db.SaveChanges();
            AuthHelper.SignIn(Response, u, false); // cập nhật lại tên trong cookie
            TempData["Success"] = "Cập nhật thông tin thành công";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) { TempData["Error"] = "Vui lòng chọn ảnh"; return RedirectToAction("Index"); }
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext) || file.ContentLength > 2 * 1024 * 1024)
            { TempData["Error"] = "Chỉ nhận ảnh jpg/png/webp tối đa 2MB"; return RedirectToAction("Index"); }

            var dir = Server.MapPath("~/Content/uploads/avatars");
            Directory.CreateDirectory(dir);
            var name = Guid.NewGuid().ToString("N") + ext;
            file.SaveAs(Path.Combine(dir, name));

            var u = db.Users.Find(User.UserId());
            u.AvatarUrl = "/Content/uploads/avatars/" + name;
            db.SaveChanges();
            TempData["Success"] = "Đã đổi ảnh đại diện";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel m)
        {
            var hasher = new PasswordHasher();
            var u = db.Users.Find(User.UserId());
            if (!ModelState.IsValid) { TempData["Error"] = "Dữ liệu đổi mật khẩu chưa hợp lệ"; return RedirectToAction("Index"); }
            if (hasher.VerifyHashedPassword(u.PasswordHash, m.OldPassword) == PasswordVerificationResult.Failed)
            { TempData["Error"] = "Mật khẩu hiện tại không đúng"; return RedirectToAction("Index"); }
            u.PasswordHash = hasher.HashPassword(m.NewPassword);
            db.SaveChanges();
            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}