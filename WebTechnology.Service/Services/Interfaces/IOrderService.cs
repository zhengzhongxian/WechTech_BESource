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

        /// <summary>
        /// Cập nhật trạng thái đơn hàng bất kỳ (chỉ dành cho admin)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="statusId">ID trạng thái mới</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        Task<ServiceResponse<bool>> AdminUpdateOrderStatusAsync(string orderId, string statusId, string token);

        Task<ServiceResponse<decimal>> CalculateOrderTotalAsync(string orderId, string token);
        Task<ServiceResponse<bool>> CancelOrderAndRestoreStockAsync(string orderId, string token);

        // Lấy lịch sử mua hàng của customer hiện tại (dựa vào token)
        Task<ServiceResponse<PaginatedResult<OrderResponseDTO>>> GetCustomerOrderHistoryAsync(OrderHistoryQueryRequest request, string token);

        // Lấy lịch sử mua hàng của một customer cụ thể (dành cho admin)
        Task<ServiceResponse<PaginatedResult<OrderResponseDTO>>> GetCustomerOrderHistoryByAdminAsync(OrderHistoryQueryRequest request, string token);

        // Lấy danh sách đơn hàng của khách hàng hiện tại có phân trang theo trạng thái
        Task<ServiceResponse<PaginatedResult<OrderResponseDTO>>> GetCustomerPaginatedOrdersAsync(OrderQueryRequest request, string token);
    }
}