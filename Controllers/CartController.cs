using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Helper;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;




        public CartController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;

            _httpContextAccessor = httpContextAccessor;

        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (currentUser == null)
            {
                return RedirectToAction("Login", "UserAuthentication");
            }

            Cart carts = _dbContext.Carts.FirstOrDefault(c => c.UserId == currentUser);

            List<CartDetail> cartDetails = null;


            if (carts != null)
            {
                cartDetails = _dbContext.CartDetails.Include(cd => cd.Product)
                    .Where(cd => cd.CartId == carts.CartId).ToList();
            }

            var viewModel = new CartViewModel
            {
                Carts = carts,

                CartDetails = cartDetails
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // Check if user is logged in
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (currentUser == null)
            {
                // If not, redirect to login view
                return RedirectToAction("Login", "UserAuthentication");
            }

            var cart = await _dbContext.Carts
               .Include(c => c.CartDetails)
               .FirstOrDefaultAsync(c => c.UserId == currentUser);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = currentUser,
                    UserId = currentUser,
                    CartDetails = new List<CartDetail>()
                };

                _dbContext.Carts.Add(cart);
            }

            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartDetail == null)
            {
                cartDetail = new CartDetail
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price,
                    TotalPrice = quantity * product.Price,
                    CartId = cart.CartId
                };
                cart.CartDetails.Add(cartDetail);
            }
            else
            {
                //cartDetail.Quantity += quantity;
                cartDetail.TotalPrice = cartDetail.Quantity * @product.Price;
                _dbContext.CartDetails.Update(cartDetail);
            }           

            // Save changes to database
            await _dbContext.SaveChangesAsync();

            //return RedirectToAction("Index");
            return RedirectToAction("Menu", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCartItem(int productId)
        {
            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.Product.Id == productId);

            if (cartDetail == null)
            {
                return NotFound();
            }

            _dbContext.CartDetails.Remove(cartDetail);

            _dbContext.SaveChanges();

            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            var cart = _dbContext.Carts.FirstOrDefault(c => c.UserId == currentUser);

            var listCartDetails = _dbContext.CartDetails.Include(cd => cd.Product)
                .Where(cd => cd.CartId == cart.CartId).ToList();

            var viewModel = new CartViewModel
            {
                Carts = cart,

                CartDetails = listCartDetails
            };

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartItem(int productId, string changeQuantity)
        {
            var cartDetail = _dbContext.CartDetails.FirstOrDefault(cd => cd.Product.Id == productId);

            if (cartDetail == null)
            {
                return NotFound();
            }

            if (changeQuantity == "increase")
            {
                cartDetail.Quantity++;
            }
            else if (changeQuantity == "decrease")
            {
                if (cartDetail.Quantity > 1)
                {
                    cartDetail.Quantity--;
                }
            }

            // Calculate new total price for cart
            var currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            var cart = _dbContext.Carts.FirstOrDefault(c => c.UserId == currentUser);

            var cartDetails = _dbContext.CartDetails.Include(cd => cd.Product).Where(cd => cd.CartId == cart.CartId).ToList();

            var totalPrice = cartDetails.Sum(cd => cd.Product.Price * cd.Quantity);

            cartDetail.TotalPrice = cartDetail.Quantity * cartDetail.Price ;


            _dbContext.SaveChanges();

            var viewModel = new CartViewModel
            {
                Carts = cart,

                CartDetails = cartDetails,
            };         

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            // Get current user from session
        
            var userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = _dbContext.Customers.FirstOrDefault(c => c.UserId == userId);

            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || cart.CartDetails.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Customer = customer,
                CartDetails = cart.CartDetails.ToList(),
                TotalPrice = cart.CartDetails.Sum(cd => cd.Price * cd.Quantity),
                PaymentMethod = "COD" // Default payment method
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
        {
            var userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (userId == null)
            {
                return RedirectToAction("Login", "UserAuthentication");
            }

            var customer = _dbContext.Customers.FirstOrDefault(c => c.UserId == userId);

            var cart = _dbContext.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || cart.CartDetails.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                TotalPrice = cart.CartDetails.Sum(cd => cd.Price * cd.Quantity),
                Status = "Pending",
                CustomerId = customer.customerId,
                OrderDate = DateTime.Now,
                //PaymentMethod = model.PaymentMethod
            };

            order.OrderDetails = new List<OrderDetail>();


            foreach (var cartItem in cart.CartDetails)
            {
                var orderDetail = new OrderDetail
                {
                    Order = order,

                    PricePerUnit = cartItem.Price,

                    Quantity = cartItem.Quantity,

                    ProductId = cartItem.ProductId,

                    Product = cartItem.Product
                };

                order.OrderDetails.Add(orderDetail);
            }

            _dbContext.Orders.Add(order);

            _dbContext.Carts.Remove(cart);

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }        
    }
}

