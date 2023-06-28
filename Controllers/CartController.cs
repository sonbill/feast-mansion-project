using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using feast_mansion_project.Helper;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    public class CartController : Controller
    {
        private List<string> generatedWords = new List<string>();
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private async Task<string> GenerateRandomNonsenseWordAsync(int length)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var random = new Random();

            var word = new StringBuilder(length);

            while (true)
            {
                for (int i = 0; i < length; i++)
                {
                    int index = random.Next(allowedChars.Length);
                    word.Append(allowedChars[index]);
                }

                // Check if the word is already generated
                if (!generatedWords.Contains(word.ToString()))
                {
                    break;
                }

                // Wait for a short period before generating another word
                await Task.Delay(10);
                word.Clear();
            }
            generatedWords.Add(word.ToString());

            return word.ToString();
        }

        private Task<int> GenerateRandomNumberAsync()
        {
            Random random = new Random();
            return Task.FromResult(random.Next(1000, 9999));
        }

        public CartController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "UserAuthentication");
            }

            Cart carts = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == currentUser);
            List<CartDetail> cartDetails = null;

            if (carts != null)
            {
                cartDetails = await _dbContext.CartDetails.Include(cd => cd.Product)
                    .Where(cd => cd.CartId == carts.CartId).ToListAsync();
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
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (currentUser == 0)
            {
                TempData["InstructionMessage"] = "Bạn cần phải đăng nhập trước";
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

            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

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
                    CartId = cart.CartId
                };
                cart.CartDetails.Add(cartDetail);
            }
            else
            {
                _dbContext.CartDetails.Update(cartDetail);
            }

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm vào giỏ hàng";

            return RedirectToAction("Menu", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCartItem(int productId)
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "UserAuthentication");
            }

            var cartDetail = await _dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.Product.ProductId == productId);

            if (cartDetail == null)
            {
                return NotFound();
            }

            _dbContext.CartDetails.Remove(cartDetail);
            await _dbContext.SaveChangesAsync();

            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == currentUser);

            var listCartDetails = await _dbContext.CartDetails.Include(cd => cd.Product)
                .Where(cd => cd.CartId == cart.CartId).ToListAsync();

            var viewModel = new CartViewModel
            {
                Carts = cart,
                CartDetails = listCartDetails
            };

            TempData["SuccessMessage"] = "Đã xoá món ăn";

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItem(int productId, string changeQuantity)
        {
            int currentUser = Convert.ToInt32(HttpContext.Session.GetString("userId"));
            if (currentUser == 0)
            {
                return RedirectToAction("Login", "UserAuthentication");
            }

            var cartDetail = await _dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.Product.ProductId == productId);

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

            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == currentUser);

            if (cart == null)
            {
                return NotFound();
            }

            var cartDetails = await _dbContext.CartDetails.Include(cd => cd.Product)
                .Where(cd => cd.CartId == cart.CartId).ToListAsync();

            _dbContext.SaveChanges();

            var viewModel = new CartViewModel
            {
                Carts = cart,
                CartDetails = cartDetails
            };

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            // Get current user from session

            var userId = Convert.ToInt32(HttpContext.Session.GetString("userId"));

            if (userId == 0)
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

            if (userId == 0)
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

            var shippingFree = 20000;

            var totalPrice = cart.CartDetails.Sum(cd => cd.Price * cd.Quantity);

            // Generate a unique OrderId
            var randomWord = await GenerateRandomNonsenseWordAsync(4);
            var randomNumber = await GenerateRandomNumberAsync();
            var orderId = "HD" + randomWord + randomNumber.ToString("D4");

            var order = new Order
            {
                OrderId = orderId,
                TotalPrice = totalPrice + shippingFree,
                Status = "Pending",
                Note = model.Note,
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.Now,
                PaymentMethod = model.PaymentMethod
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

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt hàng thành công";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ConfirmCheckout(CheckoutViewModel model)
        {
            return PartialView("_Confirmation", model);
        }

    }
}

