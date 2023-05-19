using System;
using System.ComponentModel.DataAnnotations;

namespace feast_mansion_project.Models
{
    public class CategoryViewModel 
    {
        public List<Category>? Categories { get; set; }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string? ImagePath { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}

