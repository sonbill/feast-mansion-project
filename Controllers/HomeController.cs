using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using feast_mansion_project.Models.Domain;
using feast_mansion_project.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor context;
        private readonly ApplicationDbContext _dbContext;


        public HomeController(
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext
            )
        {
            context = httpContextAccessor;
            _dbContext = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index(HomeViewModel model)
        {
            var categories = _dbContext.Categories.ToList();
            var products = _dbContext.Products.ToList();

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }

        //GET: Details Product
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _dbContext.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);


            //var categories = _dbContext.Categories.ToList();

            if (products == null)
            {
                return NotFound();
            }

            //var viewModel = new HomeViewModel
            //{
            //    Categories = categories,
            //};


            return View("ProductDetail", products);
        }
        

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile(int page = 1, int pageSize = 5)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));


            var order = _dbContext.Orders.OrderBy(c => c.OrderId);

            int totalItems = await order.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var paginatedProducts = order.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            User user = _dbContext.Users.FirstOrDefault(u => u.userId == userId);

            Customer customer = _dbContext.Customers.FirstOrDefault(c => c.UserId == userId);

            var orders = _dbContext.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.CustomerId == user.Customer.customerId)
            .ToList();

            

            if (user == null)
            {
                return NotFound();
            }

            //var user = _dbContext.Users.Include(u => u.Customer).SingleOrDefault(u => u.userId == userId);

            ProfileViewModel profileViewModel = new ProfileViewModel
            {
                User = user,

                Customer = customer,

                Orders = orders,

                TotalItems = totalItems,

                CurrentPage = page,

                PageSize = pageSize,

                TotalPages = totalPages
            }; 
            return View("Profile", profileViewModel);

        }

        [HttpGet("ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpGet("OrdersHistory")]
        public async Task<IActionResult> OrdersHistory(int page = 1, int pageSize = 5)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            var orders = _dbContext.Orders
                .OrderBy(c => c.OrderId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == userId);

            //var orders = _dbContext.Orders
            //.Include(o => o.OrderDetails)
            //.ThenInclude(od => od.Product)
            //.Where(o => o.CustomerId == userId)
            //.ToList();

            int totalItems = await orders.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedOrders = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();


            

            OrderHistoryViewModel orderHistoryViewModel = new OrderHistoryViewModel
            {
                Orders = paginatedOrders,

                TotalItems = totalItems,

                CurrentPage = page,

                PageSize = pageSize,

                TotalPages = totalPages
            };

            //var order = _dbContext.Orders.Where(o => o.CustomerId == userId).ToList();


            return View("OrersHistoryList", orderHistoryViewModel);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string searchQuery)
        {
            var products = _dbContext.Products.Include(p => p.Category).ToList();
            var categories = _dbContext.Categories.ToList();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                Products = products,
                SelectedCategoryId = 0,
                SearchQuery = searchQuery
            };

            return View("Menu", viewModel);
        }        

        [HttpGet("Menu")]
        public async Task<IActionResult> Menu(int? categoryId, HomeViewModel model)
        {
            if (model.Products == null)
            {
                var viewModel = new HomeViewModel();

                // get all categories
                viewModel.Categories = await _dbContext.Categories.ToListAsync();

                // if a category id was passed, filter products by category id
                if (categoryId.HasValue)
                {
                    viewModel.Products = await _dbContext.Products
                        .Include(p => p.Category)
                        .Where(p => p.CategoryId == categoryId.Value)
                        .ToListAsync();

                    viewModel.SelectedCategoryId = categoryId.Value;
                }
                else
                {
                    viewModel.Products = await _dbContext.Products
                        .Include(p => p.Category)
                        .ToListAsync();
                }

                return View("Menu", viewModel);
            }         


            return View("Menu", model);
        }

        //[HttpGet("Menu")]
        //public async Task<IActionResult> Menu(int? categoryId)
        //{
        //    var viewModel = new HomeViewModel();

        //    // get all categories
        //    viewModel.Categories = await _dbContext.Categories.ToListAsync();

        //    // if a category id was passed, filter products by category id
        //    if (categoryId.HasValue)
        //    {
        //        viewModel.Products = await _dbContext.Products
        //            .Include(p => p.Category)
        //            .Where(p => p.CategoryId == categoryId.Value)
        //            .ToListAsync();

        //        viewModel.SelectedCategoryId = categoryId.Value;
        //    }
        //    else
        //    {
        //        viewModel.Products = await _dbContext.Products
        //            .Include(p => p.Category)
        //            .ToListAsync();
        //    }

        //    return View("Menu", viewModel);
        //}

        [HttpGet("OrdersHistory/{orderId}")]
        public async Task<IActionResult> OrdersHistory(int orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            var order = _dbContext.Orders.Include(o => o.Customer).Include(o => o.OrderDetails).ThenInclude(od => od.Product).FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet("Feedback")]
        public IActionResult Feedback()
        {

            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            return View();
        }

        public async Task<IActionResult> Logout()
        {

            return RedirectToAction("Login","UserAuthentication");
        }
    }
}

