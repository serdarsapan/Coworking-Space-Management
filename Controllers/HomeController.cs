using System.Web.Mvc;

namespace webProgramming.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // This simply redirects the user to our Facility page when they open the app
            return RedirectToAction("Index", "Facility");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}