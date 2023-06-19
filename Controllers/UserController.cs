using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using OfficeOpenXml.Style;

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
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var users = _dbContext.Users.Include(u => u.Customer).ToList();
            var customers = _dbContext.Customers.Include(c => c.User).ToList();

            var userViewModel = new UserViewModel()
            {
                Users = users,
                Customers = customers
            };

            return View("Index", userViewModel);
        }


        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id, int page = 1, int pageSize = 5)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var customerFromDb = _dbContext.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customerFromDb == null)
            {
                return NotFound();
            }

            int totalItems = customerFromDb.Orders.Count;

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedOrders = customerFromDb.Orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var customerViewModel = new CustomerViewModel()
            {
                CustomerId = customerFromDb.CustomerId,
                FullName = customerFromDb.FullName,
                Address = customerFromDb.Address + " Tp. Hồ Chí Minh",
                Phone = customerFromDb.Phone,
                Orders = paginatedOrders,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View("Edit", customerViewModel);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, CustomerViewModel customerViewModel)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var customerFromDb = await _dbContext.Customers.FindAsync(id);

            if (customerFromDb == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", customerViewModel);
            }

            customerFromDb.FullName = customerViewModel.FullName;

            customerFromDb.Address = customerViewModel.Address;

            customerFromDb.Phone = customerViewModel.Phone;

            _dbContext.Customers.Update(customerFromDb);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCustomer(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            // Find the customer record with the given ID
            var customer = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Find the user record associated with the customer record
            var user = _dbContext.Users.FirstOrDefault(u => u.Customer != null && u.Customer.CustomerId == customer.CustomerId);

            if (user == null)
            {
                return NotFound();
            }

            // Delete the customer record
            _dbContext.Customers.Remove(customer);

            // Delete the user record
            _dbContext.Users.Remove(user);

            // Save changes to the database
            _dbContext.SaveChanges();

            return RedirectToAction("Index", "User");
        }

    }
}

