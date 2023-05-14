using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using feast_mansion_project.Repositories;
using feast_mansion_project.Models.Domain;
using Microsoft.EntityFrameworkCore;
using feast_mansion_project.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace feast_mansion_project.Controllers
{

    [Route("Admin/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public DashboardController(            
            IAccountRepository accountRepository
            )
        {            
            _accountRepository = accountRepository;

        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            // Check if user is authenticated and admin
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();

        }

    }
}

