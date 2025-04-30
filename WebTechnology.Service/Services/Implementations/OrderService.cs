using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Generations;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        private string ValidateAndGetUserId(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("Token is required");

            if (_tokenService.IsTokenExpired(token))
                throw new UnauthorizedAccessException("Token has expired");

            var userId = _tokenService.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return userId;
        }

        public async Task<ServiceResponse<OrderResponseDTO>> GetOrderByIdAsync(string id, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                var order = await _orderRepository.GetOrderDetailsAsync(id);
                if (order == null)
                    return ServiceResponse<OrderResponseDTO>.FailResponse("Order not found");

                // Check if the order belongs to the user
                if (order.CustomerId != userId)
                    return ServiceResponse<OrderResponseDTO>.FailResponse("You don't have permission to access this order");

                return ServiceResponse<OrderResponseDTO>.SuccessResponse(order);
            }
            catch (Exception ex)
            {
                return ServiceResponse<OrderResponseDTO>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<OrderResponseDTO>>> GetAllOrdersAsync(string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                var orders = await _orderRepository.GetAllAsync();
                var userOrders = orders.Where(o => o.CustomerId == userId);
                var orderDTOs = _mapper.Map<IEnumerable<OrderResponseDTO>>(userOrders);
                return ServiceResponse<IEnumerable<OrderResponseDTO>>.SuccessResponse(orderDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<OrderResponseDTO>>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<OrderResponseDTO>>> GetOrdersByUserIdAsync(string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                var orders = await _orderRepository.GetOrdersByCustomerIdAsync(userId);
                return ServiceResponse<IEnumerable<OrderResponseDTO>>.SuccessResponse(orders);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<OrderResponseDTO>>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<OrderResponseDTO>> CreateOrderAsync(OrderRequestDTO orderRequest, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                await _unitOfWork.BeginTransactionAsync();

                // Validate products and check stock
                foreach (var detail in orderRequest.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product == null)
                        return ServiceResponse<OrderResponseDTO>.FailResponse($"Product {detail.ProductId} not found");

                    if (product.Stockquantity < detail.Quantity)
                        return ServiceResponse<OrderResponseDTO>.FailResponse($"Insufficient stock for product {product.ProductName}");
                }

                var order = new Order
                {
                    Orderid = Guid.NewGuid().ToString(),
                    OrderNumber = GenerateOrderNumber.Generate(),
                    CustomerId = userId,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = orderRequest.ShippingAddress,
                    ShippingFee = orderRequest.ShippingFee,
                    ShippingCode = orderRequest.ShippingCode,
                    PaymentMethod = orderRequest.PaymentMethod,
                    Notes = orderRequest.Notes,
                    StatusId = orderRequest.StatusId,
                    CreatedAt = DateTime.UtcNow,
                    IsSuccess = false
                };

                // Create order details
                foreach (var detail in orderRequest.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid().ToString(),
                        OrderId = order.Orderid,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity
                    };
                    order.OrderDetails.Add(orderDetail);

                    // Update product stock
                    product.Stockquantity -= detail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                // Calculate total price
                order.TotalPrice = await _orderRepository.CalculateOrderTotalAsync(order.Orderid);

                await _orderRepository.AddAsync(order);
                await _unitOfWork.CommitAsync();

                var orderResponse = await _orderRepository.GetOrderDetailsAsync(order.Orderid);
                return ServiceResponse<OrderResponseDTO>.SuccessResponse(orderResponse);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<OrderResponseDTO>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<OrderResponseDTO>> UpdateOrderAsync(string id, OrderRequestDTO orderRequest, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                await _unitOfWork.BeginTransactionAsync();

                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                    return ServiceResponse<OrderResponseDTO>.FailResponse("Order not found");

                // Check if the order belongs to the user
                if (existingOrder.CustomerId != userId)
                    return ServiceResponse<OrderResponseDTO>.FailResponse("You don't have permission to update this order");

                // Update order details
                foreach (var detail in orderRequest.OrderDetails)
                {
                    var existingDetail = existingOrder.OrderDetails.FirstOrDefault(od => od.ProductId == detail.ProductId);
                    if (existingDetail != null)
                    {
                        // Update quantity
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        var quantityDiff = detail.Quantity - (existingDetail.Quantity ?? 0);
                        
                        if (product.Stockquantity < quantityDiff)
                            return ServiceResponse<OrderResponseDTO>.FailResponse($"Insufficient stock for product {product.ProductName}");

                        existingDetail.Quantity = detail.Quantity;
                        product.Stockquantity -= quantityDiff;
                        await _productRepository.UpdateAsync(product);
                    }
                    else
                    {
                        // Add new order detail
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product == null)
                            return ServiceResponse<OrderResponseDTO>.FailResponse($"Product {detail.ProductId} not found");

                        if (product.Stockquantity < detail.Quantity)
                            return ServiceResponse<OrderResponseDTO>.FailResponse($"Insufficient stock for product {product.ProductName}");

                        var orderDetail = new OrderDetail
                        {
                            OrderDetailId = Guid.NewGuid().ToString(),
                            OrderId = id,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity
                        };
                        existingOrder.OrderDetails.Add(orderDetail);

                        product.Stockquantity -= detail.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }

                // Update order properties
                existingOrder.ShippingAddress = orderRequest.ShippingAddress;
                existingOrder.ShippingFee = orderRequest.ShippingFee;
                existingOrder.ShippingCode = orderRequest.ShippingCode;
                existingOrder.PaymentMethod = orderRequest.PaymentMethod;
                existingOrder.Notes = orderRequest.Notes;
                existingOrder.StatusId = orderRequest.StatusId;

                // Recalculate total price
                existingOrder.TotalPrice = await _orderRepository.CalculateOrderTotalAsync(id);

                await _orderRepository.UpdateAsync(existingOrder);
                await _unitOfWork.CommitAsync();

                var orderResponse = await _orderRepository.GetOrderDetailsAsync(id);
                return ServiceResponse<OrderResponseDTO>.SuccessResponse(orderResponse);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<OrderResponseDTO>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteOrderAsync(string id, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                await _unitOfWork.BeginTransactionAsync();

                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return ServiceResponse<bool>.FailResponse("Order not found");

                // Check if the order belongs to the user
                if (order.CustomerId != userId)
                    return ServiceResponse<bool>.FailResponse("You don't have permission to delete this order");

                // Restore product stock
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Stockquantity += (detail.Quantity ?? 0);
                        await _productRepository.UpdateAsync(product);
                    }
                }

                var result = await _orderRepository.DeleteAsync(id);
                await _unitOfWork.CommitAsync();

                return ServiceResponse<bool>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<bool>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatusAsync(string orderId, string statusId, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ServiceResponse<bool>.FailResponse("Order not found");

                // Check if the order belongs to the user
                if (order.CustomerId != userId)
                    return ServiceResponse<bool>.FailResponse("You don't have permission to update this order");

                var result = await _orderRepository.UpdateOrderStatusAsync(orderId, statusId);
                return ServiceResponse<bool>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<decimal>> CalculateOrderTotalAsync(string orderId, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ServiceResponse<decimal>.FailResponse("Order not found");

                // Check if the order belongs to the user
                if (order.CustomerId != userId)
                    return ServiceResponse<decimal>.FailResponse("You don't have permission to access this order");

                var total = await _orderRepository.CalculateOrderTotalAsync(orderId);
                return ServiceResponse<decimal>.SuccessResponse(total);
            }
            catch (Exception ex)
            {
                return ServiceResponse<decimal>.ErrorResponse(ex.Message);
            }
        }
    }
} 