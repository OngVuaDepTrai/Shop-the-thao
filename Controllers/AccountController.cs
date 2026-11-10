using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;

using ShopTheThao.Helpers;
using ShopTheThao.Models;
using ShopTheThao.ViewModels;

namespace ShopTheThao.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShopTheThaoEntities11 db = new ShopTheThaoEntities11();
        private readonly PasswordHasher hasher = new PasswordHasher();

        [HttpGet, AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel m, string returnUrl)
        {
            if (!ModelState.IsValid) return View(m);
            var email = m.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || hasher.VerifyHashedPassword(user.PasswordHash, m.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(m);
            }
            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa, vui lòng liên hệ cửa hàng");
                return View(m);
            }

            AuthHelper.SignIn(Response, user, m.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return user.Role == "Admin"
                ? RedirectToAction("Index", "Dashboard", new { area = "Admin" })
                : RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Register() => View();

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel m)
        {
            if (!ModelState.IsValid) return View(m);
            var email = m.Email.Trim().ToLower();
            if (db.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký");
                return View(m);
            }
            var user = new User
            {
                FullName = m.FullName.Trim(),
                Email = email,
                Phone = m.Phone,
                PasswordHash = hasher.HashPassword(m.Password),
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            db.Users.Add(user);
            db.SaveChanges();
            AuthHelper.SignIn(Response, user, false);
            TempData["Success"] = "Đăng ký thành công, chào mừng bạn đến với SportHub!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public ActionResult ForgotPassword() => View();

        // Không có máy chủ email nên xác minh bằng Email + SĐT đã đăng ký
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel m)
        {
            if (!ModelState.IsValid) return View(m);
            var email = m.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Phone == m.Phone);
            if (user == null)
            {
                ModelState.AddModelError("", "Email và số điện thoại không khớp với tài khoản nào");
                return View(m);
            }
            user.PasswordHash = hasher.HashPassword(m.NewPassword);
            db.SaveChanges();
            TempData["Success"] = "Đặt lại mật khẩu thành công, hãy đăng nhập lại.";
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}