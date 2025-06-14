using Microsoft.AspNetCore.Mvc;
using FoodRestaurant.Models;

namespace FoodRestaurant.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Implement email sending logic
                TempData["Success"] = "Thank you for your message. We'll get back to you soon!";
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }
    }
}