using System;
using System.Collections.Generic;

namespace feast_mansion_project.Models
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }

        public List<Product> Products { get; set; }

        public int SelectedCategoryId { get; set; }

        public string SearchQuery { get; set; }

    }
}