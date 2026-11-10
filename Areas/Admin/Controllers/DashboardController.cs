using System.Web.Mvc;

namespace ShopTheThao.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return Content("Khu vực quản trị - Admin OK");
        }
    }
}