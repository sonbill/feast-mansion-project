using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }

        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int CartId { get; set; }

        public Cart Cart { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

    }
}

