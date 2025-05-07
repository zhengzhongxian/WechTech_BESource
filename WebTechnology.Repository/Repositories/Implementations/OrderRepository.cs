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
                .ThenInclude(odd => odd.ProductPrices)
                .FirstOrDefaultAsync(o => o.Orderid == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductPrices)
                .ToListAsync();
        }

        public IQueryable<Order> GetOrdersAsQueryable()
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductPrices)
                .AsQueryable();
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
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductPrices)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
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
                    PaymentMethodName = o.PaymentMethodNavigation.PaymentName ?? "CHƯA CÓ",
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    StatusId = o.StatusId,
                    IsSuccess = o.IsSuccess,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        ProductPrice = od.Price ?? 0,
                        Quantity = od.Quantity,
                        SubTotal = od.Quantity * (od.Price ?? 0),
                        Img = od.Product.Images.FirstOrDefault(i => i.Order == "1") != null ?
                              od.Product.Images.FirstOrDefault(i => i.Order == "1").ImageData : null
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderResponseDTO?> GetOrderDetailsAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductPrices)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
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
                    PaymentMethodName = o.PaymentMethodNavigation.PaymentName ?? "CHƯA CÓ",
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    StatusId = o.StatusId,
                    IsSuccess = o.IsSuccess,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                    {
                        OrderDetailId = od.OrderDetailId,
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        ProductPrice = od.Price ?? 0,
                        Quantity = od.Quantity,
                        SubTotal = od.Quantity * (od.Price ?? 0),
                        Img = od.Product.Images.FirstOrDefault(i => i.Order == "1") != null ?
                              od.Product.Images.FirstOrDefault(i => i.Order == "1").ImageData : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string statusId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return false;

            order.StatusId = statusId;
            if (statusId == "COMPLETED")
            {
                order.IsSuccess = true;
            }
            await UpdateAsync(order);
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(string orderId)
        {
            Console.WriteLine($"DEBUG: Calculating total for order: {orderId}");

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductPrices)
                .FirstOrDefaultAsync(o => o.Orderid == orderId);

            if (order == null)
            {
                Console.WriteLine($"DEBUG: Order not found: {orderId}");
                return 0;
            }

            Console.WriteLine($"DEBUG: Order found with {order.OrderDetails.Count} details");

            // Tính tổng tiền sản phẩm
            decimal subtotal = 0;
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Product == null)
                {
                    Console.WriteLine($"DEBUG: Product is null for detail: {detail.OrderDetailId}");
                    continue;
                }

                var productPrice = detail.Price ?? 0;
                Console.WriteLine($"DEBUG: Product {detail.ProductId} price: {productPrice}, quantity: {detail.Quantity}");
                subtotal += (productPrice * (detail.Quantity ?? 0));
            }

            Console.WriteLine($"DEBUG: Subtotal: {subtotal}, ShippingFee: {order.ShippingFee}");

            // Trả về tổng tiền bao gồm phí ship
            return subtotal + (order.ShippingFee ?? 0);
        }
    }
}