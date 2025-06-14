using Microsoft.AspNetCore.Mvc;

namespace FoodRestaurant.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userName);
            return View();
        }
    }
}
