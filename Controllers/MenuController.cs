using Microsoft.AspNetCore.Mvc;
using FoodRestaurant.Models;
using System.Collections.Generic;

namespace FoodRestaurant.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index()
        {
            var dishes = new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Butter Chicken",
                    Description = "Tender chicken in rich tomato-butter gravy",
                    Price = 349.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/butter-chicken.jpg",
                    IsSpicy = false,
                    IsVegetarian = false,
                    Rating = 5,
                    IsAvailable = true,
                    Ingredients = new[] { "Chicken", "Butter", "Tomato", "Cream", "Spices" },
                    Allergens = new[] { "Dairy" },
                    PrepTimeMinutes = 30,
                    CaloriesPerServing = 450
                },
                new Dish
                {
                    Id = 2,
                    Name = "Kadai Paneer",
                    Description = "Cottage cheese with bell peppers in spicy gravy",
                    Price = 299.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/kadai-paneer.jpg",
                    IsSpicy = true,
                    IsVegetarian = true,
                    Rating = 4,
                    IsAvailable = true,
                    Ingredients = new[] { "Paneer", "Bell Peppers", "Onion", "Tomato", "Spices" },
                    Allergens = new[] { "Dairy" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 350
                },
                new Dish
                {
                    Id = 3,
                    Name = "Fish Curry",
                    Description = "Coastal style fish curry with coconut base",
                    Price = 389.00M,
                    Category = "Main Course",
                    ImageUrl = "/images/dishes/fish-curry.jpg",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 5,
                    IsAvailable = true,
                    Ingredients = new[] { "Fish", "Coconut", "Tomato", "Spices" },
                    Allergens = new[] { "Fish" },
                    PrepTimeMinutes = 35,
                    CaloriesPerServing = 380
                },
                // Main Course - South Indian
                new Dish
                {
                    Id = 4,
                    Name = "Fish Curry",
                    Description = "Coastal style fish curry with coconut base",
                    Price = 389.00M,
                    ImageUrl = "/images/food/fish-curry.png",
                    Category = "Main Course",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 4,
                    Ingredients = new[] { "Fish", "Coconut", "Spices", "Curry Leaves" },
                    PrepTimeMinutes = 35,
                    CaloriesPerServing = 320
                },
                // Main Course - Chinese
                new Dish
                {
                    Id = 5,
                    Name = "Chilli Chicken",
                    Description = "Indo-Chinese style spicy chicken",
                    Price = 329.00M,
                    ImageUrl = "/images/food/chilli-chicken.png",
                    Category = "Main Course",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 4,
                    Ingredients = new[] { "Chicken", "Bell Peppers", "Soy Sauce", "Spices" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 350
                },
                // Main Course - Continental
                new Dish
                {
                    Id = 6,
                    Name = "Grilled Fish",
                    Description = "Herb-crusted fish with lemon butter sauce",
                    Price = 449.00M,
                    ImageUrl = "/images/food/grilled-fish.png",
                    Category = "Main Course",
                    IsSpicy = false,
                    IsVegetarian = false,
                    Rating = 5,
                    Ingredients = new[] { "Fish", "Herbs", "Butter", "Lemon" },
                    PrepTimeMinutes = 30,
                    CaloriesPerServing = 300
                },
                // Starters
                new Dish
                {
                    Id = 7,
                    Name = "Crispy Spring Rolls",
                    Description = "Vegetable stuffed crispy rolls with sweet chili sauce",
                    Price = 199.00M,
                    ImageUrl = "/images/food/spring-rolls.png",
                    Category = "Starters",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 4,
                    Ingredients = new[] { "Mixed Vegetables", "Spring Roll Wrapper", "Garlic", "Soy Sauce" },
                    PrepTimeMinutes = 20,
                    CaloriesPerServing = 280
                },
                new Dish
                {
                    Id = 8,
                    Name = "Chicken Tikka",
                    Description = "Grilled marinated chicken pieces with mint chutney",
                    Price = 289.00M,
                    ImageUrl = "/images/food/chicken-tikka.png",
                    Category = "Starters",
                    IsSpicy = true,
                    IsVegetarian = false,
                    Rating = 5,
                    Ingredients = new[] { "Chicken", "Yogurt", "Spices", "Lemon" },
                    PrepTimeMinutes = 25,
                    CaloriesPerServing = 320
                },
                // Desserts
                new Dish
                {
                    Id = 9,
                    Name = "Gulab Jamun",
                    Description = "Sweet milk dumplings in rose flavored syrup",
                    Price = 149.00M,
                    ImageUrl = "/images/food/gulab-jamun.png",
                    Category = "Desserts",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 5,
                    Ingredients = new[] { "Milk Powder", "Sugar", "Cardamom", "Rose Water" },
                    PrepTimeMinutes = 40,
                    CaloriesPerServing = 250
                },
                new Dish
                {
                    Id = 10,
                    Name = "Mango Ice Cream",
                    Description = "Creamy mango flavored ice cream with fresh fruit pieces",
                    Price = 169.00M,
                    ImageUrl = "/images/food/mango-ice-cream.png",
                    Category = "Desserts",
                    IsSpicy = false,
                    IsVegetarian = true,
                    Rating = 4,
                    Ingredients = new[] { "Mango", "Cream", "Milk", "Sugar" },
                    PrepTimeMinutes = 15,
                    CaloriesPerServing = 200
                }
            };

            return View(dishes);
        }
    }
}