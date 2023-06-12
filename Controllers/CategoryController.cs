using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/Category")]
    public class CategoryController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        private readonly ApplicationDbContext _dbContext;

        public CategoryController(
            ApplicationDbContext dbContext,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;

            _dbContext = dbContext;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var categories = _dbContext.Categories.OrderBy(c => c.Id);

            int totalItems = categories.Count();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedCategories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new CategoryViewModel
            {
                Categories = paginatedCategories,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View("~/Views/Category/Index.cshtml", viewModel);
        }

        //GET: Create Category
        [HttpGet("Create")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        //POST: Create Category 
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryViewModel obj, IFormFile thumbnail)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (obj != null)
            {
                if (thumbnail != null && thumbnail.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnail.FileName);

                    var subfolderName = "CategoryThumbnail";

                    // Get the wwwroot folder path
                    var webRootPath = _hostingEnvironment.WebRootPath;

                    // Set the folder path
                    var folderPath = Path.Combine(webRootPath, "Uploads", subfolderName);

                    // If the folder doesn't exist, create it
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Set the file path
                    var filePath = Path.Combine(folderPath, fileName);

                    // Upload file to path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await thumbnail.CopyToAsync(stream);
                    }
                    
                    obj.ImagePath = fileName;
                }                
            }            

            if (ModelState.IsValid)
            {
                try
                {
                    var category = new Category
                    {
                        Name = obj.Name,

                        Description = obj.Description,                        

                        ImagePath = obj.ImagePath
                    };

                    _dbContext.Categories.Add(category);

                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Category added successfully";

                    Console.WriteLine(JsonConvert.SerializeObject(obj));

                    return RedirectToAction("Index");

                } catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }

            }
            //Console.WriteLine(JsonConvert.SerializeObject(obj));

            return View("Create", obj);
        }


        //GET: Update Category
        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || id == 0)
            {
                return NotFound();
            }


            var categoryFromDb = _dbContext.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }


            return View("Edit", categoryFromDb);
        }

        //PUT: Update Category
            
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category obj, IFormFile? newThumbnail)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var category = await _dbContext.Categories.FindAsync(obj.Id);

            if (category == null)
            {
                return NotFound();
            }

            string oldImagePath = category.ImagePath;

            if (newThumbnail != null && newThumbnail.Length > 0)
            {
                // Generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(newThumbnail.FileName);

                var subfolderName = "CategoryThumbnail";

                // Get the wwwroot folder path
                var webRootPath = _hostingEnvironment.WebRootPath;

                // Set the folder path
                var folderPath = Path.Combine(webRootPath, "Uploads", subfolderName);

                // Set the file path
                var filePath = Path.Combine(folderPath, fileName);

                // Upload file to path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await newThumbnail.CopyToAsync(stream);
                }

                // Set product image path
                obj.ImagePath = fileName;

                // Delete old image file
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var oldFilePath = Path.Combine(folderPath, oldImagePath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            else
            {
                obj.ImagePath = oldImagePath;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Name = obj.Name;

                    category.Description = obj.Description;                    

                    category.ImagePath = obj.ImagePath;
                    //_dbContext.Categories.Update(category);

                    _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Category updated successfully";

                    return RedirectToAction("Edit", obj);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }                
            }
            return View("Edit", obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve the product from the database
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);

            if (category != null)
            {
                try
                {
                    // Delete the product from the database
                    _dbContext.Categories.Remove(category);

                    var subfolderName = "CategoryThumbnail";

                    // Set the folder path
                    var folderPath = Path.Combine("wwwroot", "Uploads", subfolderName);

                    // Set the file path
                    var oldFilePath = Path.Combine(folderPath, category.ImagePath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Category deleted successfully";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;

                    return RedirectToAction("Index");
                }
            }

            TempData["ErrorMessage"] = "Không tồn tại Danh mục này!";

            return View("Index");
        }
    }
}

