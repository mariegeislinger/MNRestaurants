using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
// ^ All the apps (libraries) that we need to use

namespace Restaurants.Pages
{
    // This PageModel controls the Razor Page logic for displaying restaurant data
    public class RestaurantsModel : PageModel
    {
        // Stores the selected Restaurant ID from the form (dropdown)
        [BindProperty]
        public int SelectedRestaurantId { get; set; }

        // List used to populate the dropdown menu with restaurants
        public List<SelectListItem> RestaurantList { get; set; }

        // Stores the selected Restaurant object (details to display)
        public Restaurant SelectedRestaurant { get; set; }

        // Runs when the page first loads (GET request)
        public void OnGet()
        {
            LoadRestaurantList(); // Fill dropdown
        }

        // Runs when the form is submitted (POST request)
        public IActionResult OnPost()
        {
            LoadRestaurantList(); // Reload dropdown (important after post)

            // If a valid restaurant is selected, get its full details
            if (SelectedRestaurantId != 0)
            {
                SelectedRestaurant = GetRestaurantById(SelectedRestaurantId);
            }

            return Page();
        }

        // Loads restaurant names and IDs from the database into the dropdown list
        private void LoadRestaurantList()
        {
            RestaurantList = new List<SelectListItem>();

            using (var connection = new SqliteConnection("Data Source=Restaurants.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Restaurants";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RestaurantList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // Restaurant ID
                            Text = reader.GetString(1)              // Restaurant Name
                        });
                    }
                }
            }
        }

        // Retrieves full details for a specific restaurant by ID
        private Restaurant GetRestaurantById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=Restaurants.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Restaurants WHERE Id = @Id";

                // Parameterized query (prevents SQL injection)
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Restaurant
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Location = reader.GetString(2),
                            Description = reader.GetString(3),
                            FoodType = reader.GetString(4),
                            ImageFileName = reader.GetString(5)
                        };
                    }
                }
            }

            // Return null if no restaurant is found
            return null;
        }
    }

    // Model class representing a single restaurant
    public class Restaurant
    {
        public int Id { get; set; }              // Unique ID
        public string Name { get; set; }         // Restaurant name
        public string Location { get; set; }     // Address or city
        public string Description { get; set; }  // Short description
        public string FoodType { get; set; }     // Cuisine type (e.g., Italian, Mexican)
        public string ImageFileName { get; set; } // Image file name/path
    }
}