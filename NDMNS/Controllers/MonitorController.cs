using Microsoft.AspNetCore.Mvc;

namespace NDMNS.Controllers
{
    public class MonitorController : BaseController
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            ViewBag.UserId = HttpContext.Session.GetString("Id");
            ViewBag.UserName = HttpContext.Session.GetString("Name");
            ViewBag.UserRole = HttpContext.Session.GetString("Role");
            ViewBag.IsNOC = HttpContext.Session.GetString("Role") == "Network Operation Center";

            return View();
        }
    }
}
