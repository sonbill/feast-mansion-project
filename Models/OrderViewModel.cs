using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace feast_mansion_project.Models
{
	public class OrderViewModel
	{
        [Key]
        public string OrderId { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        //Pagination
        public List<Order>? Orders { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}

