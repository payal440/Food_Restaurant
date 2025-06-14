using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FoodRestaurant.Models;
using FoodRestaurant.Models.ViewModels;

namespace FoodRestaurant.Controllers
{
    public class AccountController : Controller
    {
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetString("UserEmail") != null;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userInfo = StaticUserData.ValidateUser(model.Email, model.Password);
                if (userInfo.HasValue)
                {
                    // Store user info in session
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    HttpContext.Session.SetString("UserName", userInfo.Value.Name);
                    HttpContext.Session.SetString("UserRole", userInfo.Value.Role);
                    
                    // Initialize empty cart for new user session
                    HttpContext.Session.SetString($"Cart_{model.Email}", JsonSerializer.Serialize(new List<CartItem>()));
                    
                    TempData["Success"] = $"Welcome back, {userInfo.Value.Name}!";

                    // Redirect based on role
                    if (userInfo.Value.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Account");
                    }
                }
                
                ModelState.AddModelError("", "Invalid email or password");
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (StaticUserData.IsEmailRegistered(model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered");
                    return View(model);
                }

                if (StaticUserData.RegisterUser(model.Email, model.Password, model.Name))
                {
                    TempData["Success"] = "Registration successful! Please login to continue.";
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

        public IActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var userName = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Orders()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Get cart items specific to this user
            var cartJson = HttpContext.Session.GetString($"Cart_{userEmail}");
            var cartItems = string.IsNullOrEmpty(cartJson) 
                ? new List<CartItem>() 
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            var order = new Order
            {
                Id = new Random().Next(1000, 9999),
                CustomerId = userEmail,
                CustomerName = HttpContext.Session.GetString("UserName"),
                OrderDate = DateTime.Now,
                TotalAmount = cartItems?.Sum(item => item.Price * item.Quantity) ?? 0,
                Status = "Pending"
            };

            // Get existing orders from TempData
            var existingOrdersJson = TempData.Peek("AllOrders") as string;
            var allOrders = string.IsNullOrEmpty(existingOrdersJson)
                ? new List<Order>()
                : JsonSerializer.Deserialize<List<Order>>(existingOrdersJson);

            // Add order items
            var orderItems = cartItems.Select(item => new OrderItem
            {
                OrderId = order.Id,
                DishId = item.Id,
                DishName = item.Name,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();

            // Store order items
            var existingOrderItemsJson = TempData.Peek("OrderItems") as string;
            var allOrderItems = string.IsNullOrEmpty(existingOrderItemsJson)
                ? new List<OrderItem>()
                : JsonSerializer.Deserialize<List<OrderItem>>(existingOrderItemsJson);

            allOrderItems.AddRange(orderItems);
            TempData["OrderItems"] = JsonSerializer.Serialize(allOrderItems);

            // Add new order to list
            allOrders.Add(order);
            TempData["AllOrders"] = JsonSerializer.Serialize(allOrders);

            // Clear user's cart after order is placed
            HttpContext.Session.SetString($"Cart_{userEmail}", JsonSerializer.Serialize(new List<CartItem>()));

            return View(order);
        }

        public IActionResult Profile()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");

            var userName = HttpContext.Session.GetString("UserName");
            
            // Get the current user's profile data
            var model = new ProfileViewModel
            {
                FirstName = userName?.Split(' ').FirstOrDefault() ?? "",
                LastName = userName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                Email = userEmail
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Here you would update the user's profile in your database
            // For now, we'll just redirect back with a success message
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Menu()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login");
            }

            var dishes = new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Butter Chicken",
                    Description = "Tender chicken in rich tomato-butter gravy",
                    Price = 349.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/butter-chicken.jpg",
                    IsSpicy = false,
                    IsVegetarian = false,
                    Rating = 5,
                    IsAvailable = true,
                    Ingredients = new[] { "Chicken", "Butter", "Tomato", "Cream", "Spices" },
                    Allergens = new[] { "Dairy" },
                    PrepTimeMinutes = 30,
                    CaloriesPerServing = 450
                },
                new Dish
                {
                    Id = 2,
                    Name = "Kadai Paneer",
                    Description = "Cottage cheese with bell peppers in spicy gravy",
                    Price = 299.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/kadai-paneer.jpg",
                    IsSpicy = true,
                    IsVegetarian = true,
                    Rating = 4,
                    IsAvailable = true,
                    Ingredients = new[] { "Paneer", "Bell Peppers", "Onion", "Tomato", "Spices" },
                    Allergens = new[] { "Dairy" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 350
                },
                new Dish
                {
                    Id = 3,
                    Name = "Fish Curry",
                    Description = "Coastal style fish curry with coconut base",
                    Price = 389.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/fish-curry.jpg",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 5,
                    IsAvailable = true,
                    Ingredients = new[] { "Fish", "Coconut", "Tomato", "Spices" },
                    Allergens = new[] { "Fish" },
                    PrepTimeMinutes = 35,
                    CaloriesPerServing = 380
                },
                // Main Course - South Indian
                new Dish
                {
                    Id = 4,
                    Name = "Fish Curry",
                    Description = "Coastal style fish curry with coconut base",
                    Price = 389.00M,
                    ImageUrl = "/images/food/fish-curry.png",
                    Category = "Main Course",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 4,
                    Ingredients = new[] { "Fish", "Coconut", "Spices", "Curry Leaves" },
                    PrepTimeMinutes = 35,
                    CaloriesPerServing = 320
                },
                // Main Course - Chinese
                new Dish
                {
                    Id = 5,
                    Name = "Chilli Chicken",
                    Description = "Indo-Chinese style spicy chicken",
                    Price = 329.00M,
                    ImageUrl = "/images/food/chilli-chicken.png",
                    Category = "Main Course",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 4,
                    Ingredients = new[] { "Chicken", "Bell Peppers", "Soy Sauce", "Spices" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 350
                },
                // Main Course - Continental
                new Dish
                {
                    Id = 6,
                    Name = "Grilled Fish",
                    Description = "Herb-crusted fish with lemon butter sauce",
                    Price = 449.00M,
                    ImageUrl = "/images/food/grilled-fish.png",
                    Category = "Main Course",
                    IsSpicy = false,
                    IsVegetarian = false,
                    Rating = 5,
                    Ingredients = new[] { "Fish", "Herbs", "Butter", "Lemon" },
                    PrepTimeMinutes = 30,
                    CaloriesPerServing = 300
                },
                // Starters
                new Dish
                {
                    Id = 7,
                    Name = "Crispy Spring Rolls",
                    Description = "Vegetable stuffed crispy rolls with sweet chili sauce",
                    Price = 199.00M,
                    ImageUrl = "/images/food/spring-rolls.png",
                    Category = "Starters",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 4,
                    Ingredients = new[] { "Mixed Vegetables", "Spring Roll Wrapper", "Garlic", "Soy Sauce" },
                    PrepTimeMinutes = 20,
                    CaloriesPerServing = 280
                },
                new Dish
                {
                    Id = 8,
                    Name = "Chicken Tikka",
                    Description = "Grilled marinated chicken pieces with mint chutney",
                    Price = 289.00M,
                    ImageUrl = "/images/food/chicken-tikka.png",
                    Category = "Starters",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 5,
                    Ingredients = new[] { "Chicken", "Yogurt", "Spices", "Lemon" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 320
                },
                // Desserts
                new Dish
                {
                    Id = 9,
                    Name = "Gulab Jamun",
                    Description = "Sweet milk dumplings in rose flavored syrup",
                    Price = 149.00M,
                    ImageUrl = "/images/food/gulab-jamun.png",
                    Category = "Desserts",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 5,
                    Ingredients = new[] { "Milk Powder", "Sugar", "Cardamom", "Rose Water" },
                    PrepTimeMinutes = 40,
                    CaloriesPerServing = 250
                },
                new Dish
                {
                    Id = 10,
                    Name = "Mango Ice Cream",
                    Description = "Creamy mango flavored ice cream with fresh fruit pieces",
                    Price = 169.00M,
                    ImageUrl = "/images/food/mango-ice-cream.png",
                    Category = "Desserts",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 4,
                    Ingredients = new[] { "Mango", "Cream", "Milk", "Sugar" },
                    PrepTimeMinutes = 15,
                    CaloriesPerServing = 200
                }
            };

            return View(dishes);
        }

        public IActionResult Logout()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (!string.IsNullOrEmpty(userEmail))
            {
                // Clear user-specific cart
                HttpContext.Session.Remove($"Cart_{userEmail}");
            }
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been successfully logged out.";
            return RedirectToAction("Index", "Home");
        }
    }
}