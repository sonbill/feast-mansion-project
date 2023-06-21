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
using OfficeOpenXml;

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

        [HttpGet]
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

            int selectTotalOrders = _orderRepository.GetTotalOrdersByMonth(selectMonth, selectYear);
           
            int totalOrders = _orderRepository.GetTotalOrdersByMonth(currentMonth, currentYear);
            
            List<Order> orders = _orderRepository.GetOrdersByMonth(selectMonth, selectYear);

            List<OrderDashboardViewModel> orderViewModels = new List<OrderDashboardViewModel>();

            foreach (var order in orders)
            {
                var customer = _dbContext.Customers.FirstOrDefault(c => c.CustomerId == order.CustomerId);

                var orderViewModel = new OrderDashboardViewModel
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

        public IActionResult ExportToExcel(DateTime startDate, DateTime endDate)
        {
            var revenueAndOrderCountByDay = new Dictionary<DateTime, (decimal Revenue, int OrderCount)>();

            var orders = _dbContext.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete")
                .ToList();

            if (orders.Count != 0)
            {
                // Iterate over each day in the date range
                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var totalRevenue = _orderRepository.GetTotalRevenueByDaysForExcel(date, date.AddDays(1).AddSeconds(-1));
                    var orderCount = _orderRepository.GetOrderCountByDaysForExcel(date, date.AddDays(1).AddSeconds(-1));
                    revenueAndOrderCountByDay[date] = (totalRevenue, orderCount);
                }

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Create a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Total Revenue and Order Count");

                    // Set the header text
                    worksheet.Cells["A1"].Value = "Ngày";
                    worksheet.Cells["B1"].Value = "Doanh thu";
                    worksheet.Cells["C1"].Value = "Tổng đơn hàng trong ngày";

                    // Populate the data rows
                    int rowIndex = 2;
                    foreach (var entry in revenueAndOrderCountByDay)
                    {
                        worksheet.Cells[rowIndex, 1].Value = entry.Key.ToShortDateString();
                        worksheet.Cells[rowIndex, 2].Value = entry.Value.Revenue;
                        worksheet.Cells[rowIndex, 3].Value = entry.Value.OrderCount;
                        rowIndex++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells.AutoFitColumns();

                    // Convert the Excel package to a byte array
                    byte[] excelBytes = package.GetAsByteArray();

                    // Return the Excel file as a downloadable attachment
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TotalRevenue.xlsx");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không có kết quả";
                return RedirectToAction("Index", "Dashboard");
            }
        }

    }
}

