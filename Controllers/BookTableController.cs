using Microsoft.AspNetCore.Mvc;
using FoodRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace FoodRestaurant.Controllers
{
    public class BookTableController : Controller
    {
        private static List<TableBooking> _bookings = new List<TableBooking>();
        private static int _nextId = 1;

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.IsLoggedIn = !string.IsNullOrEmpty(userName);
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var booking = new TableBooking
            {
                BookingDate = DateTime.Today,
                BookingTime = DateTime.Now.Date.AddHours(19), // Default to 7 PM
                CustomerName = userName
            };

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TableBooking booking)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    booking.Id = _nextId++;
                    booking.Status = "Confirmed";
                    booking.CreatedAt = DateTime.Now;
                    _bookings.Add(booking);

                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
                    {
                        // For non-logged in users, show confirmation page
                        TempData["Success"] = "Your table has been booked successfully! Please check your email for confirmation.";
                    }
                    else
                    {
                        TempData["Success"] = "Your table has been booked successfully!";
                    }
                    return RedirectToAction("Confirmation", new { id = booking.Id });
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.Error.WriteLine($"Error processing booking: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while processing your booking.");
                    return View(booking);
                }
            }
            return View(booking);
        }

        public IActionResult Confirmation(int id)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpGet]
        public IActionResult GetAvailableTimeSlots(DateTime date)
        {
            // In a real application, this would check a database
            var availableSlots = new List<DateTime>();
            var startTime = date.Date.AddHours(11); // Restaurant opens at 11 AM
            var endTime = date.Date.AddHours(22);   // Last booking at 10 PM

            while (startTime <= endTime)
            {
                if (!_bookings.Any(b => b.BookingDate.Date == date.Date && 
                                      b.BookingTime.TimeOfDay == startTime.TimeOfDay))
                {
                    availableSlots.Add(startTime);
                }
                startTime = startTime.AddMinutes(30); // 30-minute slots
            }

            return Json(availableSlots);
        }
    }
}