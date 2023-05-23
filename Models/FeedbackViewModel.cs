using System;
using System.ComponentModel.DataAnnotations;

namespace feast_mansion_project.Models
{
	public class FeedbackViewModel
	{
        public List<Feedback>? Feedbacks { get; set; }

        [Key]
        public int FeedbackId { get; set; }

        public string OpinionOpinionState { get; set; }

        public string FeedbackCategory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }

        public int CustomerId { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}

