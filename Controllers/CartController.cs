using Microsoft.AspNetCore.Mvc;
using FoodRestaurant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FoodRestaurant.Controllers
{
    public class CartController : Controller
    {
        private List<CartItem> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> items)
        {
            var cartJson = JsonSerializer.Serialize(items);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            return View(cartItems);
        }

        public IActionResult Checkout()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Orders", "Account");
        }

        [HttpPost]
        public IActionResult Add(int id, string name, decimal price, string imageUrl = "", string description = "")
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Json(new { success = false, requiresLogin = true });
            }

            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(i => i.Id == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    Id = id,
                    Name = name,
                    Price = price,
                    ImageUrl = string.IsNullOrEmpty(imageUrl) ? "/images/dishes/default-dish.jpg" : imageUrl,
                    Description = description,
                    Quantity = 1
                });
            }

            SaveCartItems(cartItems);

            var cartTotal = cartItems.Sum(i => i.Price * i.Quantity);
            return Json(new { success = true, cartCount = cartItems.Sum(i => i.Quantity), total = cartTotal });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cartItems = GetCartItems();
            var item = cartItems.FirstOrDefault(i => i.Id == id);

            if (item != null)
            {
                item.Quantity = Math.Max(1, Math.Min(10, quantity));
                SaveCartItems(cartItems);

                var itemTotal = item.Price * item.Quantity;
                var cartTotal = cartItems.Sum(i => i.Price * i.Quantity);

                return Json(new { success = true, itemTotal, cartTotal });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var cartItems = GetCartItems();
            var item = cartItems.FirstOrDefault(i => i.Id == id);

            if (item != null)
            {
                cartItems.Remove(item);
                SaveCartItems(cartItems);

                var cartTotal = cartItems.Sum(i => i.Price * i.Quantity);
                return Json(new { success = true, cartCount = cartItems.Sum(i => i.Quantity), total = cartTotal });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Implement payment processing
                TempData["Success"] = "Payment processed successfully!";
                // Clear the cart after successful payment
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("OrderConfirmation");
            }
            return View("Checkout", model);
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }
    }
}