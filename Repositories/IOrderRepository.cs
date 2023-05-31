using System;
using System.Linq;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;
using OfficeOpenXml;

namespace feast_mansion_project.Repositories
{
    public interface IOrderRepository
    {
        decimal GetTotalRevenueByMonth(int month, int year);
        int GetTotalCompletedOrdersByMonth(int month, int year);
        int GetTotalCanceledOrdersByMonth(int month, int year);
        int GetTotalOrdersByMonth(int month, int year);
        int GetTotalCompletedOrdersByCurrentMonth(int currentMonth, int currentYear);
        int GetTotalCanceledOrdersByCurrentMonth(int currentMonth, int currentYear);
        List<Order> GetOrdersByMonth(int month, int year);
        //byte[] ExportTotalRevenueToExcel(DateTime startDate, DateTime endDate);
        decimal GetTotalRevenueByDays(DateTime startDate, DateTime endDate);
        int GetOrderCountByDays(DateTime startDate, DateTime endDate);
        decimal GetTotalRevenueByDaysForExcel(DateTime startDate, DateTime endDate);
        int GetOrderCountByDaysForExcel(DateTime startDate, DateTime endDate);


    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public decimal GetTotalRevenueByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Complete")
                .Sum(o => o.TotalPrice);
        }
        //Completed Orders
        public int GetTotalCompletedOrdersByCurrentMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Complete");
        }
        //Canceled Orders
        public int GetTotalCanceledOrdersByCurrentMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Cancel");
        }
        //Completed Orders
        public int GetTotalCompletedOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Complete");
        }
        //Canceled Orders
        public int GetTotalCanceledOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Cancel");
        }
        public int GetTotalOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year);
        }

        public List<Order> GetOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .ToList();
        }

        public decimal GetTotalRevenueByDays(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete")
                .Sum(o => o.TotalPrice);
        }

        public int GetOrderCountByDays(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete");
        }

        public decimal GetTotalRevenueByDaysForExcel(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete")
                .Sum(o => o.TotalPrice);
        }

        public int GetOrderCountByDaysForExcel(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Complete");
        }
    }
}

