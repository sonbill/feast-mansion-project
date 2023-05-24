using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class Order
	{
        [Key]
        public string OrderId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public string Status { get; set; }
        [Required]
        public string PaymentMethod { get; set; }

        public string? Note { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [ForeignKey("OrderId")]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
    }

}

