using System.Collections.Concurrent;

namespace FoodRestaurant.Models
{
    public static class StaticUserData
    {
        private static readonly ConcurrentBag<(string Email, string Password, string Name, string Role)> Users = new()
        {
            ("admin@example.com", "admin123", "Admin User", "Admin"),
            ("user@example.com", "user123", "Regular User", "User")
        };

        public static (string Name, string Role)? ValidateUser(string email, string password)
        {
            var user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != default)
            {
                return (user.Name, user.Role);
            }
            return null;
        }

        public static bool IsEmailRegistered(string email)
        {
            return Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public static bool RegisterUser(string email, string password, string name)
        {
            if (IsEmailRegistered(email))
            {
                return false;
            }

            Users.Add((email, password, name, "User"));
            return true;
        }
    }
}
