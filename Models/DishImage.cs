using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FoodRestaurant.Models
{
    public class DishImage
    {
        [Required(ErrorMessage = "Please select a dish")]
        [Display(Name = "Select Dish")]
        public int DishId { get; set; }
        
        [Required(ErrorMessage = "Please select an image file")]
        [Display(Name = "Image File")]
        public IFormFile ImageFile { get; set; } = null!;
    }
} 