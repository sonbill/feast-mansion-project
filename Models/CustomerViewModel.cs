using System;
using System.ComponentModel.DataAnnotations;

namespace feast_mansion_project.Models
{
	public class CustomerViewModel
	{
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        public ICollection<Order>? Orders { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

    }
}

