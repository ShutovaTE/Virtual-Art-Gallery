using Microsoft.AspNetCore.Mvc;

namespace Art_Gallery.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
