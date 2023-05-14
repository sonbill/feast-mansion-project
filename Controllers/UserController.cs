using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var users = _dbContext.Users.Include(u => u.Customer).ToList();
            var customers = _dbContext.Customers.Include(c => c.User).ToList();

            var userViewModel = new UserViewModel()
            {
                Users = users,
                Customers = customers
            };

            return View("Index", userViewModel);
        }
    }
}

