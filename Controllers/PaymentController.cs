using Microsoft.AspNetCore.Mvc;
using FoodRestaurant.Models;
using FoodRestaurant.Models.ViewModels;

namespace FoodRestaurant.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Process(decimal amount)
        {
            ViewBag.Amount = amount;
            return View();
        }

        [HttpPost]
        public IActionResult CompletePayment(string cardNumber, string expiryDate, string cvv)
        {
            // Simulate payment processing
            // In a real application, you would integrate with a payment gateway
            
            // Redirect to success page
            TempData["PaymentSuccess"] = true;
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            if (TempData["PaymentSuccess"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
