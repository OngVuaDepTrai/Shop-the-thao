using System.Web.Mvc;

namespace ShopTheThao.Areas.Admin.Controllers
{

    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Dashboard";

            return View();
        }
    }

}