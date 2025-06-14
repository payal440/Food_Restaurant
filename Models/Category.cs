using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodRestaurant.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Dish> Products { get; set; }

        public Category()
        {
            Products = new List<Dish>();
        }
    }
}
