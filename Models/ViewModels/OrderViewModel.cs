using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FoodRestaurant.Models;

namespace FoodRestaurant.Models.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }  // Changed from OrderId to Id to match view
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CartItem> OrderItems { get; set; }
        public string Status { get; set; } = "Pending";  // Changed from OrderStatus to Status to match view

        // Payment Information
        [Required]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Please enter a valid 16-digit card number")]
        public string? CardNumber { get; set; }

        [Required]
        [Display(Name = "Card Holder Name")]
        public string? CardHolderName { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Please enter a valid expiry date (MM/YY)")]
        public string? ExpiryDate { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Please enter a valid CVV")]
        public string? CVV { get; set; }

        // Delivery Information
        [Required]
        [Display(Name = "Delivery Address")]
        public string? DeliveryAddress { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string? ContactNumber { get; set; }
    }
}