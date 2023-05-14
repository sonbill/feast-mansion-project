using System;
using feast_mansion_project.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace feast_mansion_project.Models
{
	public class User
    {
        [Key]
        public int userId { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public bool IsAdmin { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
   
        public Customer ?Customer { get; set; }
	}
}

