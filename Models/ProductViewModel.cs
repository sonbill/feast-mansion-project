using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace feast_mansion_project.Models
{
	public class ProductViewModel
	{
        //public Product Product { get; set; }
        public List<Product>? Products { get; set; }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên món ăn.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mã SKU.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mô tả.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá món ăn.")]
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập giá món ăn.")]
        public int CategoryId { get; set; }

        //PAGINATION

        public List<Category>? ListCategory { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        //public IEnumerable<SelectListItem> Categories { get; set; }
    }
}

