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
                .FirstOrDefaultAsync(p => p.ProductId == id);


            //var categories = _dbContext.Categories.ToList();

            if (products == null)
            {
                return NotFound();
            }           


            return View("ProductDetail", products);
        }      

        

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            User user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            Customer customer = _dbContext.Customers.FirstOrDefault(c => c.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            //var user = _dbContext.Users.Include(u => u.Customer).SingleOrDefault(u => u.userId == userId);

            ProfileViewModel profileViewModel = new ProfileViewModel
            {
                CustomerId = customer.CustomerId,

                FullName = customer.FullName,

                Address = customer.Address,

                Phone = customer.Phone,

                Email = user.Email

            };

            return View("Profile", profileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve the existing customer record from the database
            var customer = await _dbContext.Customers.FindAsync(userId);

            if (ModelState.IsValid)
            {
                // Update the customer's properties with the values from the form
                customer.CustomerId = model.CustomerId;

                customer.FullName = model.FullName;

                customer.Phone = model.Phone;

                customer.Address = model.Address;

                // Save the changes to the database
                await _dbContext.SaveChangesAsync();

                //TempData["SuccessMessage"] = "Cập nhập thông tin thành công";

                // Optionally, you can redirect the user to a success page
                return RedirectToAction("Profile");
            }

            //TempData["ErrorMessage"] = "Cập nhập thông tin không thành công";

            // If the form data is not valid, return to the profile page to display errors
            return View("Profile", model);
        }



        [HttpGet("ChangePassword")]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpGet("OrdersHistory")]
        public async Task<IActionResult> OrdersHistory(int page = 1, int pageSize = 5)
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            var orders = _dbContext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate);            

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


            return View("OrdersHistoryList", orderHistoryViewModel);
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

        [HttpGet("OrdersHistory/{orderId}")]
        public async Task<IActionResult> OrdersHistoryDetail(string orderId)
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

            return View("OrdersHistory", order);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrderFromCustomer(string orderId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = _dbContext.Orders.Find(orderId);

            order.Status = "Cancel";

            _dbContext.Orders.Update(order);

            await _dbContext.SaveChangesAsync();            

            return RedirectToAction("OrdersHistory", new { orderId });
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

        [HttpPost]
        public async Task<IActionResult> SendFeedback(Feedback obj)
        {
            if (HttpContext.Session.GetString("userId") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (ModelState.IsValid)
            {
                try
                {
                    obj.CustomerId = userId;

                    obj.Status = "chưa đọc";

                    // Add new product to database
                    _dbContext.Feedbacks.Add(obj);

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công";

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }
            }
            return View("Feedback");
        }

        public async Task<IActionResult> Logout()
        {

            return RedirectToAction("Login","UserAuthentication");
        }
    }
}

