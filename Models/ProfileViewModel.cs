using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace feast_mansion_project.Models
{
	public class ProfileViewModel
	{

        //public User User { get; set; }

        //public Customer Customer { get; set; }


        //public IEnumerable<Order> Orders { get; set; }

        //public int TotalItems { get; set; }

        //public int CurrentPage { get; set; }

        //public int PageSize { get; set; }

        //public int TotalPages { get; set; }

        public int CustomerId { get; set; }
        [Display(Name = "Tên đầy đủ")]
        [Required(ErrorMessage = "Vui lòng nhập tên đầy đủ.")]
        public string FullName { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string Address { get; set; }
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        public string? Email { get; set; }      


    }
}

