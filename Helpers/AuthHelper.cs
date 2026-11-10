using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using ShopTheThao.Models;

namespace ShopTheThao.Helpers
{
    public static class AuthHelper
    {
        public static void SignIn(HttpResponseBase response, User u, bool remember)
        {
            var expire = DateTime.Now.AddDays(remember ? 14 : 1);
            var ticket = new FormsAuthenticationTicket(1, u.Email, DateTime.Now, expire, remember,
                u.Id + "|" + u.Role + "|" + u.FullName);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            { HttpOnly = true };
            if (remember) cookie.Expires = expire;
            response.Cookies.Add(cookie);
        }

        static string[] Data(IPrincipal p)
        {
            var id = p?.Identity as FormsIdentity;
            return id?.Ticket.UserData?.Split('|');
        }
        public static int UserId(this IPrincipal p) { var d = Data(p); return d != null ? int.Parse(d[0]) : 0; }
        public static string UserRole(this IPrincipal p) { var d = Data(p); return d != null ? d[1] : ""; }
        public static string FullName(this IPrincipal p) { var d = Data(p); return d != null ? d[2] : ""; }
    }
}