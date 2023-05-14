using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace feast_mansion_project.Models.DTO
{
	public class RegisterViewModel
	{
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        //[Required]
        //public string FirstName { get; set; } = null!;
        //[Required]
        //public string LastName { get; set; } = null!;
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; } = null!;

        //{
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
        //}
        //[Required]
        //public string Password { get; set; } = null!;
        //[Required]
        //[Compare("Password")]
        //public string PasswordConfirm { get; set; } = null!;
    }
}

//[Required]
//[EmailAddress]
//[Display(Name = "Email")]
//public string Email { get; set; }

//[Required]
//[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
//[DataType(DataType.Password)]
//[Display(Name = "Password")]
//public string Password { get; set; }

//[DataType(DataType.Password)]
//[Display(Name = "Confirm password")]
//[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//public string ConfirmPassword { get; set; }
