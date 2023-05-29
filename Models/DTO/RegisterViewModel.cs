using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace feast_mansion_project.Models.DTO
{
	public class RegisterViewModel
	{
        [Required(ErrorMessage = "Yêu cầu nhập Email.")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu.")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Yêu cầu xác nhận mật khẩu.")]
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập họ tên.")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ.")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại.")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

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
