using Microsoft.AspNetCore.Mvc;

namespace HRSystem.MVC
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Login","HR");
        }
    }
}
