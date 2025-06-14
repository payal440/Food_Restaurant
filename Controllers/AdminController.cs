using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodRestaurant.Services;
using FoodRestaurant.Models;
using FoodRestaurant.Models.ViewModels;

namespace FoodRestaurant.Controllers
{
    public class AdminController : Controller
    {
        private class DashboardStats
        {
            public int TotalOrders { get; set; }
            public decimal TotalRevenue { get; set; }
            public int TotalProducts { get; set; }
            public int TotalCustomers { get; set; }
            public List<TopProduct> TopProducts { get; set; }
        }

        public class TopProduct
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public int Sales { get; set; }
            public decimal Revenue { get; set; }
        }

        private static readonly List<(int Id, string Name, string Category, decimal Price, string ImageUrl, string Description)> _dishes = new List<(int, string, string, decimal, string, string)>();

        private static readonly List<Order> _orders = new List<Order>();
        private static readonly List<OrderItem> _orderItems = new List<OrderItem>();
        private static readonly List<TableBooking> _tableBookings = new List<TableBooking>();

        private readonly ImageService _imageService;

        public AdminController(ImageService imageService)
        {
            _imageService = imageService;
        }

        // Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                TempData["Error"] = "You must be logged in as an administrator to access this area.";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return Unauthorized();

            try
            {
                // Get orders from session
                var ordersJson = TempData.Peek("AllOrders") as string;
                var allOrders = string.IsNullOrEmpty(ordersJson)
                    ? new List<Order>()
                    : JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

                var completedOrders = allOrders.Where(o => o.Status == "Completed").ToList();

                // Get cart items for order details
                var cartJson = HttpContext.Session.GetString("Cart");
                var cartItems = string.IsNullOrEmpty(cartJson)
                    ? new List<CartItem>()
                    : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();

                var stats = new DashboardStats
                {
                    TotalOrders = allOrders.Count,
                    TotalRevenue = completedOrders.Sum(o => o.TotalAmount),
                    TotalProducts = _dishes.Count,
                    TotalCustomers = allOrders.Select(o => o.CustomerName).Distinct().Count(),
                    TopProducts = cartItems
                        .GroupBy(ci => new { ci.DishId, ci.DishName })
                        .Select(g => new TopProduct
                        {
                            Name = g.Key.DishName,
                            Category = _dishes.FirstOrDefault(d => d.Id == g.Key.DishId).Category ?? "Unknown",
                            Sales = g.Sum(ci => ci.Quantity),
                            Revenue = g.Sum(ci => ci.Price * ci.Quantity)
                        })
                        .OrderByDescending(x => x.Sales)
                        .Take(5)
                        .ToList()
                };

                // Add recent orders
                var recentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new {
                        o.Id,
                        o.CustomerName,
                        o.OrderDate,
                        o.TotalAmount,
                        o.Status,
                        Items = cartItems
                            .Where(ci => ci.OrderId == o.Id)
                            .Select(ci => new {
                                ci.DishName,
                                ci.Quantity,
                                ci.Price
                            }).ToList()
                    })
                    .ToList();

                return Json(new { 
                    stats = stats,
                    recentOrders = recentOrders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Products Management
        public IActionResult Products()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            return View(_dishes);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(IFormFile imageFile, string name, string category, decimal price, string description)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category) || price <= 0)
                {
                    TempData["Error"] = "Please fill in all required fields correctly.";
                    return View();
                }

                string imageUrl = "/images/dishes/default.jpg";
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                        var uploadsFolder = Path.Combine(_imageService.GetWebRootPath(), "images", "dishes");
                        
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        imageUrl = $"/images/dishes/{uniqueFileName}";
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error saving image: {ex.Message}";
                        return View();
                    }
                }

                var newId = _dishes.Any() ? _dishes.Max(d => d.Id) + 1 : 1;
                _dishes.Add((newId, name, category, price, imageUrl, description));

                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding product: " + ex.Message;
                return View();
            }
        }

        // Orders Management
        public IActionResult Orders()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            var ordersJson = TempData.Peek("AllOrders") as string;
            var allOrders = string.IsNullOrEmpty(ordersJson)
                ? new List<Order>()
                : JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

            // Get order items
            var orderItemsJson = TempData.Peek("OrderItems") as string;
            var allOrderItems = string.IsNullOrEmpty(orderItemsJson)
                ? new List<OrderItem>()
                : JsonSerializer.Deserialize<List<OrderItem>>(orderItemsJson) ?? new List<OrderItem>();

            var orders = allOrders.Select(o => {
                // Calculate total from order items
                var orderTotal = allOrderItems
                    .Where(oi => oi.OrderId == o.Id)
                    .Sum(oi => oi.Price * oi.Quantity);

                return (
                    OrderId: o.Id,
                    CustomerName: o.CustomerName,
                    OrderDate: o.OrderDate,
                    Amount: orderTotal,
                    Status: o.Status ?? "Pending"
                );
            }).ToList();

            // Group order items by order ID for display
            var orderItems = allOrderItems
                .Select(oi => (
                    OrderId: oi.OrderId,
                    DishName: oi.DishName,
                    Quantity: oi.Quantity,
                    Price: oi.Price,
                    Total: oi.Price * oi.Quantity
                ))
                .ToList();

            ViewBag.OrderItems = orderItems;
            ViewBag.TotalRevenue = orders.Sum(o => o.Amount);

            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var ordersJson = TempData["AllOrders"] as string;
                var allOrders = JsonSerializer.Deserialize<List<Order>>(ordersJson);

                var order = allOrders.FirstOrDefault(o => o.Id == orderId);
                if (order != null)
                {
                    order.Status = status;
                    TempData["AllOrders"] = JsonSerializer.Serialize(allOrders);
                    return Json(new { success = true, message = $"Order #{orderId} status updated to {status}" });
                }
                return Json(new { success = false, message = "Order not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Customers Management
        public IActionResult Customers()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            var customers = (_orders ?? new List<Order>()).Select(o => o.CustomerName).Distinct().ToList();
            
            var orderDetails = (_orders ?? new List<Order>()).Select(o => new {
                OrderId = o.Id,
                o.CustomerName,
                o.OrderDate,
                Amount = o.TotalAmount,
                Status = o.Status ?? "Pending",
                Items = (_orderItems ?? new List<OrderItem>()).Where(oi => oi.OrderId == o.Id)
                    .Select(oi => new {
                        oi.DishName,
                        oi.Quantity,
                        oi.Price,
                        Total = oi.Price * oi.Quantity
                    }).ToList()
            }).ToList();

            ViewBag.OrderDetails = orderDetails;
            return View(customers);
        }

        // Categories Management
        public IActionResult Categories()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

            var categories = _dishes.Select(d => d.Category)
                .Distinct()
                .Select(c => new Category { 
                    Name = c,
                    Description = "",
                    Products = _dishes.Where(d => d.Category == c)
                        .Select(d => new Dish {
                            Id = d.Id,
                            Name = d.Name,
                            Category = d.Category,
                            Price = d.Price,
                            ImageUrl = d.ImageUrl,
                            Description = d.Description
                        })
                        .ToList()
                })
                .ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var productIndex = _dishes.FindIndex(d => d.Id == id);
                if (productIndex >= 0)
                {
                    _dishes.RemoveAt(productIndex);
                    return Json(new { success = true, message = "Product deleted successfully" });
                }
                return Json(new { success = false, message = "Product not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Logout
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        // Print Order
        public IActionResult PrintOrder(int id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return RedirectToAction("Login", "Account");

                var ordersJson = TempData.Peek("AllOrders") as string;
            var allOrders = string.IsNullOrEmpty(ordersJson)
                ? new List<Order>()
                : JsonSerializer.Deserialize<List<Order>>(ordersJson) ?? new List<Order>();

            var order = allOrders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            var cartJson = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                UserId = order.CustomerId,
                OrderDate = order.OrderDate,
                OrderItems = cartItems,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };

            return View(orderViewModel);
        }

        // Table Booking Management
        [HttpGet]
        public IActionResult GetTableBookings()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return Unauthorized();

            try
            {
                var bookings = _tableBookings
                    .OrderByDescending(b => b.BookingDate)
                    .ThenByDescending(b => b.BookingTime)
                    .Take(10)  // Get latest 10 bookings
                    .Select(b => new
                    {
                        id = b.Id,
                        customerName = b.CustomerName,
                        date = b.BookingDate.ToString("yyyy-MM-dd"),
                        time = b.BookingTime.ToString("HH:mm"),
                        numberOfGuests = b.NumberOfGuests,
                        status = b.Status,
                        email = b.Email,
                        phone = b.PhoneNumber,
                        specialRequests = b.SpecialRequests
                    })
                    .ToList();

                return Json(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateBookingStatus([FromBody] BookingStatusUpdate model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
                return Unauthorized();

            try
            {
                var booking = _tableBookings.FirstOrDefault(b => b.Id == model.BookingId);
                if (booking == null)
                    return NotFound(new { success = false, message = "Booking not found" });

                booking.Status = model.Status;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public class BookingStatusUpdate
        {
            public int BookingId { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}