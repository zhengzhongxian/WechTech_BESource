using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly WebTech _context;

        public OrderRepository(WebTech context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(string id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Orderid == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> FindAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Order> AddAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Order> UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var order = await GetByIdAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders.AnyAsync(predicate);
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetOrdersByCustomerIdAsync(string customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.Orderid,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                    ShippingAddress = o.ShippingAddress,
                    ShippingFee = o.ShippingFee,
                    ShippingCode = o.ShippingCode,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    StatusId = o.StatusId,
                    IsSuccess = o.IsSuccess,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        ProductPrice = od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive)!.Price,
                        Quantity = od.Quantity,
                        SubTotal = od.Quantity * od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive)!.Price
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderResponseDTO?> GetOrderDetailsAsync(string orderId)
        {
            return await _context.Orders
                .Where(o => o.Orderid == orderId)
                .Select(o => new OrderResponseDTO
                {
                    OrderId = o.Orderid,
                    OrderNumber = o.OrderNumber,
                    CustomerId = o.CustomerId,
                    OrderDate = o.OrderDate,
                    ShippingAddress = o.ShippingAddress,
                    ShippingFee = o.ShippingFee,
                    ShippingCode = o.ShippingCode,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    StatusId = o.StatusId,
                    IsSuccess = o.IsSuccess,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        ProductPrice = od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive)!.Price,
                        Quantity = od.Quantity,
                        SubTotal = od.Quantity * od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive)!.Price
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string statusId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return false;

            order.StatusId = statusId;
            await UpdateAsync(order);
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(string orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return 0;

            decimal total = 0;
            foreach (var detail in order.OrderDetails)
            {
                var productPrice = detail.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive)?.Price ?? 0;
                total += (productPrice * (detail.Quantity ?? 0));
            }

            return total + (order.ShippingFee ?? 0);
        }
    }
} 