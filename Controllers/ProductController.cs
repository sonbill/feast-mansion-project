using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/Product")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;

        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {           
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var products = _dbContext.Products.OrderBy(c => c.Id);

            int totalItems = await products.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ProductViewModel
            {
                Products = paginatedProducts,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

       
        //GET: Create Product
        [HttpGet("Create")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new ProductViewModel
            {
                ListCategory = _dbContext.Categories.ToList(),
            };


            return View("~/Views/Product/Create.cshtml", viewModel);
        }

        //POST: Create Product

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model, IFormFile userFile)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (userFile != null && userFile.Length > 0)
            {
                // Generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userFile.FileName);

                // Set file path
                var filePath = Path.Combine("wwwroot", "Uploads", fileName);

                // Upload file to path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userFile.CopyToAsync(stream);
                }

                // Set product image path
                model.ImagePath = fileName;

            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = new Product
                    {
                        Name = model.Name,

                        Description = model.Description,

                        Price = model.Price,

                        CategoryId = model.CategoryId,

                        SKU = model.SKU,

                        ImagePath = model.ImagePath
                    };

                    // Add new product to database
                    _dbContext.Products.Add(product);

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product added successfully";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }

            }

            Console.WriteLine(JsonConvert.SerializeObject(model));          

            return View("Create", model);
        }


        // GET: Edit Product
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int? id, string productImage)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var product = _dbContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

            var productFromDb = _dbContext.Products.Find(id);


            if (product == null)
            {
                return NotFound();
            }

            var categories = _dbContext.Categories.ToList();

            ViewData["Categories"] = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View("Edit", productFromDb);
        }

        // POST: Edit Product
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product model, IFormFile newImage)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var product = await _dbContext.Products.FindAsync(model.Id);

            if (product == null)
            {
                return NotFound();
            }

            string oldImagePath = product.ImagePath;

            if (newImage != null && newImage.Length > 0)
            {
                // Generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(newImage.FileName);

                // Set file path
                var filePath = Path.Combine("wwwroot", "Uploads", fileName);

                // Upload file to path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                // Set product image path
                model.ImagePath = fileName;

                // Delete old image file
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var oldFilePath = Path.Combine("wwwroot", "Uploads", oldImagePath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                model.ImagePath = oldImagePath;
            }
           
            if (product != null)
            {
                try
                {
                    product.Name = model.Name;

                    product.Description = model.Description;

                    product.Price = model.Price;

                    product.CategoryId = model.CategoryId;

                    product.SKU = model.SKU;

                    product.ImagePath = model.ImagePath;

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product updated successfully";

                    return RedirectToAction("Index", model);

                } catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }

            }
            return RedirectToAction("Index", model);
        }      

        // POST: Delete Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id )
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve the product from the database
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {               
                try
                {
                    // Delete the product from the database
                    _dbContext.Products.Remove(product);

                    // Delete the product image from the wwwroot folder
                    //var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", product.ImagePath);
                    var oldFilePath = Path.Combine("wwwroot", "Uploads", product.ImagePath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product deleted successfully";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error deleting product: " + ex.Message;

                    return RedirectToAction("Index");
                }
            }

            return View("Index");
        }
    }
}

