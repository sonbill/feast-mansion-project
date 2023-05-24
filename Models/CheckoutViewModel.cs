using System;
namespace feast_mansion_project.Models
{
	public class CheckoutViewModel
	{
        public Customer Customer { get; set; }

        public List<CartDetail> CartDetails { get; set; }        

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public string PaymentMethod { get; set; }
    }
}

