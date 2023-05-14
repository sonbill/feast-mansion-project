using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class Cart
	{
        [Key]
        public int CartId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        //[ForeignKey("CartDetailId")]
        //public ICollection<CartDetail> CartDetails { get; set; }

        [ForeignKey("CartId")]
        public ICollection<CartDetail> CartDetails { get; set; }

    }
}

