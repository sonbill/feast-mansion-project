using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feast_mansion_project.Models
{
	public class Feedback
	{
        [Key]
        public int FeedbackId { get; set; }

        public string OpinionState { get; set; }

        public string FeedbackCategory { get; set; }

        public string FeedbackMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}

