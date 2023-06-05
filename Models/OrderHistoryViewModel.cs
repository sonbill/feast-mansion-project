using System;
namespace feast_mansion_project.Models
{
	public class OrderHistoryViewModel
	{
        public List<Order> Orders { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}

