using System.ComponentModel.DataAnnotations;

namespace FoodRestaurant.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int DishId { get; set; }

        [Required]
        public string DishName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public Order Order { get; set; } = null!;
        public Dish Dish { get; set; } = null!;
    }
}
