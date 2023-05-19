using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class OrderDetail
	{
        [Key]
        public int OrderDetailId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PricePerUnit { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string OrderId { get; set; }

        public Order ?Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product ?Product { get; set; }
    }
}

