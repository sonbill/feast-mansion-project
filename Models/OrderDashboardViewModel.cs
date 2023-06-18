using System;
using System.ComponentModel.DataAnnotations;

namespace feast_mansion_project.Models
{
	public class OrderDashboardViewModel
	{
        [Key]
        public string OrderId { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }
    }
}

