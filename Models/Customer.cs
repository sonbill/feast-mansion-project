using System;
using feast_mansion_project.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class Customer
	{
        [Key]
        public int customerId { get; set; }

		public string FullName { get; set; }

		public string Address { get; set; }

		public string Phone { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Order> Orders { get; set; }

        public ICollection<Cart> Carts { get; set; }

        [ForeignKey("UserId")]
        public int ?UserId { get; set; }

		public User ?User { get; set; }
    }
}

