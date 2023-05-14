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



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/Category")]
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
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

            //IEnumerable<Category> objCategoryList = _dbContext.Categories.ToList();


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
        public IActionResult CreateCategory(Category obj)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Categories.Add(obj);

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
        [Route("Edit/{id}")]
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


            return View(categoryFromDb);
        }

        //PUT: Update Category
            
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory(int id, Category obj)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Categories.Update(obj);

                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Category updated successfully";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                }                
            }
            return View(obj);
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

