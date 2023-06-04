using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/Order")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[HttpGet("Search")]
        //public async Task<IActionResult> Search(string searchQuery)
        //{
        //    var orders = _dbContext.Orders.ToList();

        //    if (!string.IsNullOrWhiteSpace(searchQuery))
        //    {
        //        orders = orders.Where(p =>
        //            p.OrderId.Contains(searchQuery, StringComparison.OrdinalIgnoreCase).ToList();
        //    }

        //    var viewModel = new HomeViewModel
        //    {
        //        Categories = categories,
        //        SelectedCategoryId = 0,
        //        SearchQuery = searchQuery
        //    };

        //    return View("Menu", viewModel);
        //}

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            //var orders = _dbContext.Orders.OrderBy(o => o.OrderId);

            //var orders = _dbContext.Orders.Include(o => o.Customer).OrderBy(o => o.OrderId);
            var orders = _dbContext.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate);

            int totalItems = await orders.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new OrderViewModel
            {
                Orders = paginatedOrders,

                TotalItems = totalItems,

                CurrentPage = page,

                PageSize = pageSize,

                TotalPages = totalPages
            };
            
            return View("Index", viewModel);
        }

        // GET: Edit Order
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var order = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost("approve/{orderId}")]
        public async Task<IActionResult> ApproveOrder(string orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var order = await _dbContext.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Update order status to "Cooking"
            order.Status = "Cooking";

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("complete-cooking/{orderId}")]
        public async Task<IActionResult> CompleteCooking(string orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.Find(orderId);

            if (order == null)
            {
                // handle error
                return NotFound();
            }

            if (order.Status != "Cooking")
            {
                // handle error
                return NotFound();
            }

            order.Status = "Shipping";

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("complete-shipping/{orderId}")]
        public async Task<IActionResult> CompleteShipping(string orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.Find(orderId);
            if (order == null)
            {
                // handle error
                return NotFound();
            }

            if (order.Status != "Shipping")
            {
                // handle error
                return NotFound();
            }

            order.Status = "Complete";

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.Find(orderId);

            order.Status = "Cancel";

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();            

            return RedirectToAction("Index");            
        }       

    }
}

