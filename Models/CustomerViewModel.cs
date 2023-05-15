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
    }
}

