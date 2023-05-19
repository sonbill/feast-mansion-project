using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using feast_mansion_project.Models.Domain;

namespace feast_mansion_project.Models
{
	public class Category
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        [ForeignKey("CategoryId")]
        public ICollection<Product> ?Products { get; set; }

        //public Category()
        //{
        //    Products = new List<Product>();
        //}

    }


}

