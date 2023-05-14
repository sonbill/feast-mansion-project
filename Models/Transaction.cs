using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class Transaction
	{
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public string PaymentType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public string Status { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}

