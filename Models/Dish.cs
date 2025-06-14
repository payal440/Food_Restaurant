using System;
using System.ComponentModel.DataAnnotations;

namespace FoodRestaurant.Models
{
    public class Dish
    {
        public const string DEFAULT_IMAGE_PATH = "/images/dishes/default-dish.jpg";

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        private string? _imageUrl;
        public string ImageUrl 
        { 
            get => string.IsNullOrEmpty(_imageUrl) ? DEFAULT_IMAGE_PATH : _imageUrl;
            set => _imageUrl = value;
        }

        public bool IsSpicy { get; set; }
        public bool IsVegetarian { get; set; }
        public int Rating { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string[] Ingredients { get; set; } = Array.Empty<string>();
        public string[] Allergens { get; set; } = Array.Empty<string>();
        public int PrepTimeMinutes { get; set; }
        public int CaloriesPerServing { get; set; }
    }
}