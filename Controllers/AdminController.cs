using System.Web.Mvc;

namespace Petal_Express_PH.Controllers
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isAdmin = Session["is_admin"] as bool?;
            if (isAdmin != true)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}
