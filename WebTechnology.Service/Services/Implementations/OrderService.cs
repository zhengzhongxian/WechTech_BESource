using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Orders;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Extensions;
using WebTechnology.Service.CoreHelpers.Generations;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IApplyVoucherRepository _applyVoucherRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IVoucherRepository voucherRepository,
            IApplyVoucherRepository applyVoucherRepository,
            IProductPriceRepository productPriceRepository,
            IImageRepository imageRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _voucherRepository = voucherRepository;
            _applyVoucherRepository = applyVoucherRepository;
            _productPriceRepository = productPriceRepository;
            _imageRepository = imageRepository;
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
                var orderDTOs = _mapper.Map<IEnumerable<OrderResponseDTO>>(orders);


                return ServiceResponse<IEnumerable<OrderResponseDTO>>.SuccessResponse(orderDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<OrderResponseDTO>>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<PaginatedResult<OrderResponseDTO>>> GetPaginatedOrdersAsync(OrderQueryRequest request, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);

                // Get orders as queryable
                var query = _orderRepository.GetOrdersAsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.CustomerId))
                {
                    query = query.Where(o => o.CustomerId == request.CustomerId);
                }

                if (!string.IsNullOrEmpty(request.StatusId))
                {
                    query = query.Where(o => o.StatusId == request.StatusId);
                }

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(o =>
                        o.OrderNumber.Contains(request.SearchTerm) ||
                        o.ShippingAddress.Contains(request.SearchTerm) ||
                        o.Notes.Contains(request.SearchTerm));
                }

                if (request.StartDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    // Add one day to include the end date fully
                    var endDatePlusOneDay = request.EndDate.Value.AddDays(1);
                    query = query.Where(o => o.OrderDate < endDatePlusOneDay);
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.SortBy.ToLower() switch
                    {
                        "orderdate" => request.SortAscending
                            ? query.OrderBy(o => o.OrderDate)
                            : query.OrderByDescending(o => o.OrderDate),
                        "totalprice" => request.SortAscending
                            ? query.OrderBy(o => o.TotalPrice)
                            : query.OrderByDescending(o => o.TotalPrice),
                        "ordernumber" => request.SortAscending
                            ? query.OrderBy(o => o.OrderNumber)
                            : query.OrderByDescending(o => o.OrderNumber),
                        "status" => request.SortAscending
                            ? query.OrderBy(o => o.StatusId)
                            : query.OrderByDescending(o => o.StatusId),
                        _ => request.SortAscending
                            ? query.OrderBy(o => o.OrderDate)
                            : query.OrderByDescending(o => o.OrderDate)
                    };
                }
                else
                {
                    // Default sorting by OrderDate descending
                    query = query.OrderByDescending(o => o.OrderDate);
                }

                // Project to DTO and paginate
                var paginatedResult = await query
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
                            ProductPrice = od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true).Price ?? 0,
                            Quantity = od.Quantity,
                            SubTotal = od.Quantity * (od.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true).Price ?? 0),
                            Img = od.Product.Images.FirstOrDefault(i => i.Order == "1") != null ?
                                  od.Product.Images.FirstOrDefault(i => i.Order == "1").ImageData : null,
                        }).ToList()
                    })
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);



                return ServiceResponse<PaginatedResult<OrderResponseDTO>>.SuccessResponse(paginatedResult);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<OrderResponseDTO>>.ErrorResponse(ex.Message);
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

                    // Lấy giá sản phẩm từ ProductPriceRepository
                    var productPriceDTO = await _productPriceRepository.GetProductPriceAsync(detail.ProductId);
                    decimal productPrice = productPriceDTO.PriceIsActive;

                    var orderDetail = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid().ToString(),
                        OrderId = order.Orderid,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        Price = productPrice
                    };
                    order.OrderDetails.Add(orderDetail);
                    // Update product stock
                    product.Stockquantity -= detail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                // Thêm đơn hàng vào DbContext
                await _orderRepository.AddAsync(order);

                // Lưu vào cơ sở dữ liệu để đảm bảo có thể truy vấn
                await _unitOfWork.SaveChangesAsync();

                // Tính tổng tiền sản phẩm (chưa áp dụng voucher)
                decimal productTotal = 0;

                // Tính trực tiếp từ giá trong OrderDetail
                foreach (var detail in order.OrderDetails)
                {
                    try
                    {
                        // Sử dụng giá đã lưu trong OrderDetail
                        decimal productPrice = detail.Price ?? 0;

                        Console.WriteLine($"DEBUG: Direct calculation - Product {detail.ProductId} price: {productPrice}, quantity: {detail.Quantity}");
                        productTotal += (productPrice * (detail.Quantity ?? 0));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DEBUG: Error calculating product price: {ex.Message}");
                    }
                }

                Console.WriteLine($"DEBUG: Product Total calculated directly: {productTotal}");

                // Áp dụng voucher nếu có
                if (orderRequest.VoucherCodes != null && orderRequest.VoucherCodes.Any())
                {
                    decimal totalDiscount = 0;

                    foreach (var voucherCode in orderRequest.VoucherCodes)
                    {
                        // Tìm voucher theo mã
                        var vouchers = await _voucherRepository.FindAsync(v => v.Code == voucherCode && v.IsActive == true);
                        var voucher = vouchers.FirstOrDefault();

                        if (voucher == null) continue;

                        // Kiểm tra điều kiện áp dụng voucher
                        if (voucher.MinOrder.HasValue && productTotal < voucher.MinOrder.Value)
                            continue;

                        if (voucher.StartDate > DateTime.UtcNow || voucher.EndDate < DateTime.UtcNow)
                            continue;

                        if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
                            continue;

                        // Tính giảm giá
                        decimal discount = 0;
                        if (voucher.DiscountType == DiscountType.Percentage)
                        {
                            // Giảm giá theo phần trăm
                            discount = productTotal * (voucher.DiscountValue ?? 0) / 100;

                            // Áp dụng giới hạn giảm giá tối đa nếu có
                            if (voucher.MaxDiscount.HasValue && discount > voucher.MaxDiscount.Value)
                            {
                                discount = voucher.MaxDiscount.Value;
                            }
                        }
                        else if (voucher.DiscountType == DiscountType.FixedAmount)
                        {
                            // Giảm giá cố định
                            discount = voucher.DiscountValue ?? 0;
                        }

                        // Áp dụng voucher vào đơn hàng
                        await _applyVoucherRepository.ApplyVoucherToOrderAsync(order.Orderid, voucher.Voucherid);

                        // Cộng dồn giảm giá
                        totalDiscount += discount;
                    }

                    // Đảm bảo tổng giảm giá không vượt quá tổng tiền sản phẩm
                    if (totalDiscount > productTotal)
                    {
                        totalDiscount = productTotal;
                    }

                    // Tính tổng tiền sau khi áp dụng voucher
                    decimal finalTotal = productTotal - totalDiscount;

                    // Cập nhật tổng tiền đơn hàng (đã bao gồm phí ship)
                    order.TotalPrice = finalTotal + (order.ShippingFee ?? 0);
                    Console.WriteLine($"DEBUG: Final total with vouchers: {order.TotalPrice}");
                }
                else
                {
                    // Nếu không có voucher, tổng tiền = tổng sản phẩm + phí ship
                    order.TotalPrice = productTotal + (order.ShippingFee ?? 0);
                    Console.WriteLine($"DEBUG: Final total without vouchers: {order.TotalPrice}");
                }

                // Cập nhật đơn hàng với tổng tiền mới
                await _orderRepository.UpdateAsync(order);

                // Lưu vào cơ sở dữ liệu
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

                        // Lấy giá sản phẩm từ ProductPriceRepository
                        var productPriceDTO = await _productPriceRepository.GetProductPriceAsync(detail.ProductId);
                        decimal productPrice = productPriceDTO.PriceIsActive;

                        var orderDetail = new OrderDetail
                        {
                            OrderDetailId = Guid.NewGuid().ToString(),
                            OrderId = id,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            Price = productPrice
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

                // Tính tổng tiền sản phẩm (chưa áp dụng voucher)
                decimal productTotal = 0;

                // Tính trực tiếp từ giá trong OrderDetail
                foreach (var detail in existingOrder.OrderDetails)
                {
                    try
                    {
                        // Sử dụng giá đã lưu trong OrderDetail
                        decimal productPrice = detail.Price ?? 0;

                        Console.WriteLine($"DEBUG: Update - Product {detail.ProductId} price: {productPrice}, quantity: {detail.Quantity}");
                        productTotal += (productPrice * (detail.Quantity ?? 0));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DEBUG: Error calculating product price: {ex.Message}");
                    }
                }

                Console.WriteLine($"DEBUG: Update - Product Total calculated directly: {productTotal}");

                // Cập nhật tổng tiền đơn hàng (đã bao gồm phí ship)
                existingOrder.TotalPrice = productTotal + (existingOrder.ShippingFee ?? 0);
                Console.WriteLine($"DEBUG: Update - Final total: {existingOrder.TotalPrice}");

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
                await _unitOfWork.BeginTransactionAsync();

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ServiceResponse<bool>.FailResponse("Không tìm thấy đơn hàng");

                // Chuyển đổi statusId thành enum để dễ so sánh
                var newStatus = statusId.ToOrderStatusType();
                var currentStatus = order.StatusId.ToOrderStatusType();

                // Nếu đang cập nhật sang trạng thái CANCELLED (hủy đơn hàng)
                if (newStatus == OrderStatusType.CANCELLED)
                {
                    // Kiểm tra nếu đơn hàng đã hoàn thành hoặc đang giao hàng thì không cho phép hủy
                    if (currentStatus == OrderStatusType.COMPLETED || currentStatus == OrderStatusType.SHIPPING)
                        return ServiceResponse<bool>.FailResponse("Không thể hủy đơn hàng đã hoàn thành hoặc đang giao hàng");

                    // Hoàn lại số lượng tồn kho cho từng sản phẩm trong đơn hàng
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product != null)
                        {
                            product.Stockquantity += (detail.Quantity ?? 0);
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                }

                // Cập nhật trạng thái đơn hàng
                order.StatusId = statusId;
                if (newStatus == OrderStatusType.COMPLETED)
                {
                    order.IsSuccess = true;

                    // Tăng số lượng đã bán (SoldQuantity) cho từng sản phẩm trong đơn hàng
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product != null)
                        {
                            // Khởi tạo SoldQuantity nếu chưa có giá trị
                            product.SoldQuantity = product.SoldQuantity ?? 0;
                            // Tăng SoldQuantity theo số lượng trong đơn hàng
                            product.SoldQuantity += detail.Quantity ?? 0;
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                }
                else if (newStatus == OrderStatusType.CANCELLED)
                {
                    order.IsSuccess = false;
                }
                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.CommitAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái đơn hàng thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
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

        public async Task<ServiceResponse<bool>> CancelOrderAndRestoreStockAsync(string orderId, string token)
        {
            try
            {
                var userId = ValidateAndGetUserId(token);
                await _unitOfWork.BeginTransactionAsync();

                // Lấy thông tin đơn hàng
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return ServiceResponse<bool>.FailResponse("Không tìm thấy đơn hàng");

                // Kiểm tra xem đơn hàng có thuộc về người dùng hiện tại không
                if (order.CustomerId != userId)
                    return ServiceResponse<bool>.FailResponse("Bạn không có quyền hủy đơn hàng này");

                // Sử dụng OrderStatusHelper để chuyển đổi từ string sang enum
                var currentStatus = order.StatusId.ToOrderStatusType();

                // Kiểm tra trạng thái đơn hàng, chỉ cho phép hủy đơn hàng ở trạng thái PENDING hoặc CONFIRMED
                if (currentStatus != OrderStatusType.PENDING && currentStatus != OrderStatusType.CONFIRMED)
                    return ServiceResponse<bool>.FailResponse("Chỉ có thể hủy đơn hàng ở trạng thái chờ xác nhận hoặc đã xác nhận");

                // Kiểm tra thêm nếu đơn hàng đã hoàn thành hoặc đang giao hàng thì không cho phép hủy
                if (currentStatus == OrderStatusType.COMPLETED || currentStatus == OrderStatusType.SHIPPING)
                    return ServiceResponse<bool>.FailResponse("Không thể hủy đơn hàng đã hoàn thành hoặc đang giao hàng");

                // Hoàn lại số lượng tồn kho cho từng sản phẩm trong đơn hàng
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdAsync(detail.ProductId);
                    if (product != null)
                    {
                        product.Stockquantity += (detail.Quantity ?? 0);
                        await _productRepository.UpdateAsync(product);
                    }
                }

                // Cập nhật trạng thái đơn hàng thành CANCELLED sử dụng enum
                order.StatusId = OrderStatusType.CANCELLED.ToOrderStatusIdString();
                order.IsSuccess = false;

                await _orderRepository.UpdateAsync(order);
                await _unitOfWork.CommitAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Đơn hàng đã được hủy thành công và số lượng tồn kho đã được hoàn lại");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi hủy đơn hàng: {ex.Message}");
            }
        }
    }
}