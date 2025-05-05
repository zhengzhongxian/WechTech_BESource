using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Orders;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(string id);
        Task<IEnumerable<Order>> GetAllAsync();
        IQueryable<Order> GetOrdersAsQueryable();
        Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate);
        Task<Order> AddAsync(Order entity);
        Task<Order> UpdateAsync(Order entity);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate);
        Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerIdAsync(string customerId);
        Task<OrderResponseDTO?> GetOrderDetailsAsync(string orderId);
        Task<bool> UpdateOrderStatusAsync(string orderId, string statusId);
        Task<decimal> CalculateOrderTotalAsync(string orderId);
    }
}