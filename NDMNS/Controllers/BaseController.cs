using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NDMNS.Controllers
{
    public abstract class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            SetCommonViewBagData();
        }

        protected void SetCommonViewBagData()
        {
            ViewBag.UserId = HttpContext.Session.GetString("Id");
            ViewBag.UserName = HttpContext.Session.GetString("Name");
            ViewBag.UserRole = HttpContext.Session.GetString("Role");
            ViewBag.IsNOC = HttpContext.Session.GetString("Role") == "Network Operation Center";
            ViewBag.IsTeamLeader = HttpContext.Session.GetString("Role") == "Team Leader";
        }
    }
}
