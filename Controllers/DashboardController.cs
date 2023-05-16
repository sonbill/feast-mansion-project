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
        private readonly IOrderRepository _orderRepository;

        public DashboardController(            
            IAccountRepository accountRepository,
            IOrderRepository orderRepository
            )
        {            
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;

        }

        // GET: /<controller>/
        //public async Task<IActionResult> Index()
        //{
        //    // Check if user is authenticated and admin
        //    if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();

        //}

        public IActionResult Index()
        {
            // Get the current month and year
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Get the total revenue for the current month
            decimal currentMonthRevenue = _orderRepository.GetTotalRevenueByMonth(currentMonth, currentYear);

            // Get the total revenue for the previous month
            decimal previousMonthRevenue = _orderRepository.GetTotalRevenueByMonth(currentMonth - 1, currentYear);

            // Get the total number of completed and canceled orders for the current month
            int completedOrders = _orderRepository.GetTotalCompletedOrdersByMonth(currentMonth, currentYear);
            int canceledOrders = _orderRepository.GetTotalCanceledOrdersByMonth(currentMonth, currentYear);

            // Pass the data to the view
            ViewData["CurrentMonthRevenue"] = currentMonthRevenue;
            ViewData["PreviousMonthRevenue"] = previousMonthRevenue;
            ViewData["CompletedOrders"] = completedOrders;
            ViewData["CanceledOrders"] = canceledOrders;

            return View();
        }

    }
}

