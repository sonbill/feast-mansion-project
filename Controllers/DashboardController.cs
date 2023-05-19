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
        private readonly ApplicationDbContext _dbContext;


        public DashboardController(            
            IAccountRepository accountRepository,
            IOrderRepository orderRepository,
            ApplicationDbContext dbContext
            )
        {            
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _dbContext = dbContext;


        }       

        public IActionResult Index(int selectMonth = 0, int selectYear = 0)
        {
            if (HttpContext.Session.GetString("UserId") == null || HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Home");
            }
            // Get the current month and year
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Get the total revenue for the current month

            decimal selectedMonthRevenue = _orderRepository.GetTotalRevenueByMonth(selectMonth, selectYear);

            decimal currentMonthRevenue = _orderRepository.GetTotalRevenueByMonth(currentMonth, currentYear);

            // Get the total revenue for the previous month
            decimal previousMonthRevenue = _orderRepository.GetTotalRevenueByMonth(currentMonth - 1, currentYear);

            decimal profit = currentMonthRevenue - previousMonthRevenue;

            int selectCompletedOrders = _orderRepository.GetTotalCompletedOrdersByMonth(selectMonth, selectYear);

            int selectCanceledOrders = _orderRepository.GetTotalCanceledOrdersByMonth(selectMonth, selectYear);

            int currentCompletedOrders = _orderRepository.GetTotalCompletedOrdersByCurrentMonth(currentMonth, currentYear);

            int currentCanceledOrders = _orderRepository.GetTotalCanceledOrdersByCurrentMonth(currentMonth, currentYear);

            int previousCompletedOrders = _orderRepository.GetTotalCompletedOrdersByCurrentMonth(currentMonth - 1, currentYear);

            int previousCanceledOrders = _orderRepository.GetTotalCompletedOrdersByCurrentMonth(currentMonth - 1, currentYear);

            int numberOfCompletedOders = currentCompletedOrders - previousCompletedOrders;

            int numberOfCanceledOders = currentCanceledOrders - previousCanceledOrders;



            // Get the total number of completed and canceled orders for the current month


            int selectTotalOrders = _orderRepository.GetTotalOrdersByMonth(selectMonth, selectYear);
           
            int totalOrders = _orderRepository.GetTotalOrdersByMonth(currentMonth, currentYear);
            
            List<Order> orders = _orderRepository.GetOrdersByMonth(selectMonth, selectYear);

            List<OrderViewModel> orderViewModels = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var customer = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                var orderViewModel = new OrderViewModel
                {
                    OrderId = order.OrderId,

                    CustomerId = customer.CustomerId,

                    OrderDate = order.OrderDate,

                    Status = order.Status,

                    TotalPrice = order.TotalPrice                    

                };

                orderViewModels.Add(orderViewModel);
            }

            // Pass the data to the view
            ViewData["CurrentMonthRevenue"] = currentMonthRevenue;
            ViewData["SelectedMonthRevenue"] = selectedMonthRevenue;
            ViewData["PreviousMonthRevenue"] = previousMonthRevenue;
            ViewData["CompletedOrders"] = currentCompletedOrders;
            ViewData["CanceledOrders"] = currentCanceledOrders;
            ViewData["SelectCanceledOrders"] = selectCanceledOrders;
            ViewData["SelectCompletedOrders"] = selectCompletedOrders;
            ViewData["SelectTotalOrders"] = selectTotalOrders;
            ViewData["TotalOrders"] = totalOrders;
            ViewData["Profit"] = profit;
            ViewData["PreviousCompletedOrders"] = previousCompletedOrders;
            ViewData["PreviousCanceledOrders"] = previousCanceledOrders;
            ViewData["NumberOfCompletedOrders"] = numberOfCompletedOders;
            ViewData["NumberOfCanceledOrders"] = numberOfCanceledOders;
            ViewData["OrderViewModels"] = orderViewModels;

            return View();
        }        

    }
}

