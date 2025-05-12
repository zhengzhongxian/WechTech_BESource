using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResponse<OrderResponseDTO>> GetOrderByIdAsync(string id, string token);
        Task<ServiceResponse<IEnumerable<OrderResponseDTO>>> GetAllOrdersAsync(string token);
        Task<ServiceResponse<PaginatedResult<OrderResponseDTO>>> GetPaginatedOrdersAsync(OrderQueryRequest request, string token);
        Task<ServiceResponse<IEnumerable<OrderResponseDTO>>> GetOrdersByUserIdAsync(string token);
        Task<ServiceResponse<OrderResponseDTO>> CreateOrderAsync(OrderRequestDTO orderRequest, string token);
        Task<ServiceResponse<OrderResponseDTO>> UpdateOrderAsync(string id, OrderRequestDTO orderRequest, string token);
        Task<ServiceResponse<bool>> DeleteOrderAsync(string id, string token);
        Task<ServiceResponse<bool>> UpdateOrderStatusAsync(string orderId, string statusId, string token);
        Task<ServiceResponse<decimal>> CalculateOrderTotalAsync(string orderId, string token);
        Task<ServiceResponse<bool>> CancelOrderAndRestoreStockAsync(string orderId, string token);
    }
}