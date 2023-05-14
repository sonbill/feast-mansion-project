using System;
namespace feast_mansion_project.Models
{
	public class CartViewModel
	{
        //public IEnumerable<Product> Product { get; set; }

        //public IEnumerable<CartDetail> CartDetails { get; set; }

        public Cart Carts { get; set; }

        public List<CartDetail> ?CartDetails { get; set; }

        public Product Product { get; set; }

    }
}

