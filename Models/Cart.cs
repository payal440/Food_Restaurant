using System.Collections.Generic;

namespace FoodRestaurant.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
    }
}
