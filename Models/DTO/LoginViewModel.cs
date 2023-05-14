using System;
using System.ComponentModel.DataAnnotations;

namespace feast_mansion_project.Models.DTO
{
	public class LoginViewModel
	{
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

//[Required]
//[EmailAddress]
//[Display(Name = "Email")]
//public string Email { get; set; }

//[Required]
//[DataType(DataType.Password)]
//[Display(Name = "Password")]
//public string Password { get; set; }

//[Display(Name = "Remember me?")]
//public bool RememberMe { get; set; }
