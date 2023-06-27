﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        //[StringLength(50)]
        [Required]
        public string SKU { get; set; }

        //[StringLength(100)]
        [Required]
        public string Name { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string IsPin { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //public Product()
        //{
        //    Category = new Category();
        //}

        [ForeignKey("ProductId")]
        public ICollection<OrderDetail>OrderDetails { get; set; }

        //[ForeignKey("Products")]
        //public ICollection<CartDetail>CartDetails { get; set; }
    }
}
