using Microsoft.AspNetCore.Mvc;

namespace Virtual_Art_Gallery.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Publications()
        {
            return View();
        }
    }
}
