using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodRestaurant.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Status { get; set; } // Pending, Preparing, Ready, Delivered

        public string PaymentStatus { get; set; } // Pending, Completed

        public string PaymentMethod { get; set; } = string.Empty;

        public ICollection<OrderItem> OrderItems { get; set; }

        public User Customer { get; set; } = null!;

        public Order()
        {
            OrderDate = DateTime.Now;
            Status = "Pending";
            PaymentStatus = "Pending";
            OrderItems = new List<OrderItem>();
        }
    }
}
