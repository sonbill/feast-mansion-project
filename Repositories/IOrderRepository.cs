using System;
using feast_mansion_project.Models;
using feast_mansion_project.Models.Domain;

namespace feast_mansion_project.Repositories
{
    public interface IOrderRepository
    {
        decimal GetTotalRevenueByMonth(int month, int year);
        int GetTotalCompletedOrdersByMonth(int month, int year);
        int GetTotalCanceledOrdersByMonth(int month, int year);
        //List<Order> GetOrdersByMonth(int month, int year);


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
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .Sum(o => o.TotalPrice);
        }

        public int GetTotalCompletedOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Complete");
        }

        public int GetTotalCanceledOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Count(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.Status == "Cancel");
        }

        public List<Order> GetOrdersByMonth(int month, int year)
        {
            return _dbContext.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year)
                .ToList();
        }
    }
}

