using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using ShopTheThao.Models;

namespace ShopTheThao.Helpers
{
    public static class ProductImage
    {
        const string Folder = "~/Content/images/products/";
        static readonly string[] Exts = { ".jpg", ".jpeg", ".png", ".webp" };

        static string Find(string name)
        {
            foreach (var e in Exts)
            {
                var vp = Folder + name + e;
                if (File.Exists(HttpContext.Current.Server.MapPath(vp)))
                    return VirtualPathUtility.ToAbsolute(vp);
            }
            return null;
        }

        static string Safe(string code)
        {
            return string.IsNullOrWhiteSpace(code) ? null : Regex.Replace(code.Trim(), @"[^A-Za-z0-9_-]", "");
        }

        // Ảnh chính: ưu tiên ImageUrl nếu có, không thì tìm file theo mã (SP001.jpg)
        public static string For(Product p)
        {
            if (!string.IsNullOrEmpty(p.ImageUrl)) return p.ImageUrl;
            var c = Safe(p.Code);
            return c == null ? null : Find(c);
        }

        // Thư viện ảnh: SP001.jpg, SP001-1.jpg ... SP001-8.jpg
        public static List<string> Gallery(Product p)
        {
            var list = new List<string>();
            var main = For(p);
            if (main != null) list.Add(main);
            var c = Safe(p.Code);
            if (c == null) return list;
            for (int i = 1; i <= 8; i++)
            {
                var u = Find(c + "-" + i);
                if (u != null) list.Add(u);
            }
            return list;
        }
    }
}