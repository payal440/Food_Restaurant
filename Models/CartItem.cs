using System.ComponentModel.DataAnnotations;

namespace FoodRestaurant.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int DishId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public virtual Dish? Dish { get; set; }
    }
}
