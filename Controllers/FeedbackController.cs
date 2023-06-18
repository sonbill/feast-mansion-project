using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{
    [Route("Admin/Feedback")]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public FeedbackController(
            ApplicationDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        // GET: /<controller>/
        [HttpGet("Index")]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            //var feedbacks = _dbContext.Feedbacks.OrderBy(c => c.FeedbackId);

            var feedbacks = _dbContext.Feedbacks.Include(o => o.Customer).OrderBy(c => c.FeedbackId);

            int totalItems = feedbacks.Count();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var paginatedFeedbacks = feedbacks.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new FeedbackViewModel
            {
                Feedbacks = paginatedFeedbacks,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };            

            return View(viewModel);
        }

        [HttpGet("Detail/{id}")]
        public IActionResult Detail(int? id)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            var feedbackDetail = _dbContext.Feedbacks.Include(f => f.Customer).FirstOrDefault(f => f.FeedbackId == id);


            return View(feedbackDetail);
        }

    }
}

