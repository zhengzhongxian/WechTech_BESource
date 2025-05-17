using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Review;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITokenService _tokenService;
        private readonly IOrderRepository _orderRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ITokenService tokenService,
            IOrderRepository orderRepository)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _tokenService = tokenService;
            _orderRepository = orderRepository;
        }

        public async Task<ServiceResponse<ReviewDTO>> CreateReview(CreateReviewDTO review, string token)
        {
            var serviceResponse = new ServiceResponse<ReviewDTO>();
            try
            {
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Token đã hết hạn");
                }

                var customerId = _tokenService.GetUserIdFromToken(token);
                if (customerId == null)
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Không tìm thấy thông tin người dùng");
                }

                var product = await _productRepository.GetByIdAsync(review.ProductId);
                if (product == null)
                {
                    return ServiceResponse<ReviewDTO>.NotFoundResponse("Sản phẩm không tồn tại");
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return ServiceResponse<ReviewDTO>.NotFoundResponse("Không tìm thấy thông tin khách hàng");
                }

                // Kiểm tra xem khách hàng đã mua sản phẩm và đơn hàng đã hoàn thành chưa
                var hasCompletedOrderResponse = await HasCompletedOrder(customerId, review.ProductId);
                if (!hasCompletedOrderResponse.Success || !hasCompletedOrderResponse.Data)
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Bạn chỉ có thể đánh giá sản phẩm khi đã mua và đơn hàng đã hoàn thành");
                }

                // Kiểm tra xem khách hàng đã đánh giá sản phẩm này chưa
                var existingReview = await _reviewRepository.GetAllAsync();
                var hasReviewed = existingReview.Any(r => r.Customerid == customerId && r.Productid == review.ProductId);
                if (hasReviewed)
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Bạn đã đánh giá sản phẩm này rồi");
                }

                var newReview = new Review
                {
                    Reviewid = Guid.NewGuid().ToString(),
                    Customerid = customerId,
                    Productid = review.ProductId,
                    Rate = review.Rate
                };

                await _reviewRepository.AddAsync(newReview);

                // Tính trung bình rate cho sản phẩm
                var allProductReviews = await _reviewRepository.GetByPropertyAsync(x => x.Productid, review.ProductId);
                // Thêm review mới vào danh sách để tính trung bình
                var reviewsWithNewOne = allProductReviews.ToList();
                reviewsWithNewOne.Add(newReview);

                // Tính trung bình rate
                if (reviewsWithNewOne.Any())
                {
                    double averageRate = reviewsWithNewOne.Average(r => r.Rate ?? 0);
                    // Làm tròn đến 1 chữ số thập phân
                    product.Rate = (int)Math.Round(averageRate);
                    await _productRepository.UpdateAsync(product);
                }

                await _unitOfWork.SaveChangesAsync();

                var reviewDto = _mapper.Map<ReviewDTO>(newReview);
                string customerName = customer.Surname + " " + customer.Middlename + " " + customer.Firstname;
                reviewDto.CustomerName = customerName;
                reviewDto.CreatedAt = DateTime.Now;

                return ServiceResponse<ReviewDTO>.SuccessResponse(reviewDto, "Đánh giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<ReviewDTO>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }
                public async Task<ServiceResponse<List<ReviewDTO>>> GetProductReviews(string productId)
        {
            var serviceResponse = new ServiceResponse<List<ReviewDTO>>();
            try
            {
                var reviews = await _reviewRepository.GetByPropertyAsync(x => x.Productid, productId);
                var reviewDtos = new List<ReviewDTO>();

                foreach (var review in reviews)
                {
                    var reviewDto = _mapper.Map<ReviewDTO>(review);
                    var customer = await _customerRepository.GetByIdAsync(review.Customerid);
                    string customerName = customer.Surname + " " + customer.Middlename + " " + customer.Firstname;
                    reviewDto.CustomerName = customerName ?? "Khách hàng";
                    reviewDto.CreatedAt = DateTime.Now; // Tạm thời, nên lưu CreatedAt vào database
                    reviewDtos.Add(reviewDto);
                }

                return ServiceResponse<List<ReviewDTO>>.SuccessResponse(reviewDtos, "Lấy danh sách đánh giá thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<ReviewDTO>>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PaginatedResult<ReviewDTO>>> GetPaginatedProductReviews(ReviewQueryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ProductId))
                {
                    return ServiceResponse<PaginatedResult<ReviewDTO>>.FailResponse("ID sản phẩm không được để trống");
                }

                // Lấy tất cả review của sản phẩm
                var reviews = await _reviewRepository.GetByPropertyAsync(x => x.Productid, request.ProductId);

                // Lọc theo số sao nếu có
                if (request.RateFilter.HasValue)
                {
                    reviews = reviews.Where(r => r.Rate == request.RateFilter.Value).ToList();
                }

                // Tính tổng số review
                var totalCount = reviews.Count();

                // Sắp xếp
                IEnumerable<Review> sortedReviews;
                if (request.SortBy.ToLower() == "rate")
                {
                    sortedReviews = request.SortAscending
                        ? reviews.OrderBy(r => r.Rate)
                        : reviews.OrderByDescending(r => r.Rate);
                }
                else // Mặc định sắp xếp theo CreatedAt
                {
                    // Tạm thời sắp xếp theo ID vì chưa có CreatedAt
                    sortedReviews = request.SortAscending
                        ? reviews.OrderBy(r => r.Reviewid)
                        : reviews.OrderByDescending(r => r.Reviewid);
                }

                // Phân trang
                var pagedReviews = sortedReviews
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                // Map sang DTO
                var reviewDtos = new List<ReviewDTO>();
                foreach (var review in pagedReviews)
                {
                    var reviewDto = _mapper.Map<ReviewDTO>(review);
                    var customer = await _customerRepository.GetByIdAsync(review.Customerid);
                    string customerName = customer.Surname + " " + customer.Middlename + " " + customer.Firstname;
                    reviewDto.CustomerName = customerName ?? "Khách hàng";
                    reviewDto.CreatedAt = DateTime.Now; // Tạm thời, nên lưu CreatedAt vào database

                    // Lấy comments nếu có
                    if (review.Comments != null && review.Comments.Any())
                    {
                        reviewDto.Comments = _mapper.Map<List<CommentDTO>>(review.Comments);
                    }

                    reviewDtos.Add(reviewDto);
                }

                // Tạo metadata phân trang
                var paginationMetadata = new PaginationMetadata(
                    request.PageNumber,
                    request.PageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<ReviewDTO>(
                    reviewDtos,
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<ReviewDTO>>.SuccessResponse(paginatedResult, "Lấy danh sách đánh giá thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<ReviewDTO>>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> DeleteReview(string reviewId, string token)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                var customerId = _tokenService.GetUserIdFromToken(token);
                if (customerId == null)
                {
                    return ServiceResponse<string>.FailResponse("Không tìm thấy thông tin người dùng");
                }

                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<string>.FailResponse("Token đã hết hạn");
                }

                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy đánh giá");
                }

                if (review.Customerid != customerId)
                {
                    return ServiceResponse<string>.FailResponse("Bạn không có quyền xóa đánh giá này");
                }

                // Lấy thông tin sản phẩm trước khi xóa review
                var productId = review.Productid;
                var product = await _productRepository.GetByIdAsync(productId);

                // Xóa review
                await _reviewRepository.DeleteAsync(reviewId);

                // Tính lại trung bình rate cho sản phẩm
                var remainingReviews = await _reviewRepository.GetByPropertyAsync(x => x.Productid, productId);

                if (remainingReviews.Any())
                {
                    // Tính trung bình rate mới
                    double averageRate = remainingReviews.Average(r => r.Rate ?? 0);
                    // Làm tròn và cập nhật
                    product.Rate = (int)Math.Round(averageRate);
                }
                else
                {
                    // Nếu không còn review nào, đặt rate về 0
                    product.Rate = 0;
                }

                // Cập nhật sản phẩm
                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<string>.SuccessResponse("Xóa đánh giá thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }
        public async Task<ServiceResponse<CommentDTO>> AddComment(CreateCommentDTO comment, string token)
        {
            try
            {
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<CommentDTO>.FailResponse("Token đã hết hạn");
                }

                var customerId = _tokenService.GetUserIdFromToken(token);
                if (customerId == null)
                {
                    return ServiceResponse<CommentDTO>.FailResponse("Không tìm thấy thông tin người dùng");
                }

                // Kiểm tra review có tồn tại không
                var review = await _reviewRepository.GetByIdAsync(comment.ReviewId);
                if (review == null)
                {
                    return ServiceResponse<CommentDTO>.NotFoundResponse("Không tìm thấy đánh giá");
                }

                // Tạo comment mới
                var newComment = new Comment
                {
                    Commentid = Guid.NewGuid().ToString(),
                    Reviewid = comment.ReviewId,
                    CommentText = comment.CommentText,
                    CommentedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                };

                // Thêm comment vào review
                review.Comments.Add(newComment);
                await _unitOfWork.SaveChangesAsync();

                // Map sang DTO
                var commentDto = _mapper.Map<CommentDTO>(newComment);

                return ServiceResponse<CommentDTO>.SuccessResponse(commentDto, "Thêm bình luận thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<CommentDTO>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> DeleteComment(string commentId, string token)
        {
            try
            {
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<string>.FailResponse("Token đã hết hạn");
                }

                var customerId = _tokenService.GetUserIdFromToken(token);
                if (customerId == null)
                {
                    return ServiceResponse<string>.FailResponse("Không tìm thấy thông tin người dùng");
                }

                // Tìm review và comment
                var reviews = await _reviewRepository.GetAllAsync();
                var review = reviews.FirstOrDefault(r => r.Comments.Any(c => c.Commentid == commentId));

                if (review == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy bình luận hoặc đánh giá");
                }

                var comment = review.Comments.FirstOrDefault(c => c.Commentid == commentId);
                if (comment == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy bình luận");
                }

                // Kiểm tra quyền xóa (chỉ người tạo review hoặc admin mới có quyền xóa comment)
                if (review.Customerid != customerId)
                {
                    return ServiceResponse<string>.FailResponse("Bạn không có quyền xóa bình luận này");
                }

                // Xóa comment khỏi collection
                review.Comments.Remove(comment);
                await _reviewRepository.UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<string>.SuccessResponse("Xóa bình luận thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> HasCompletedOrder(string token, string productId)
        {
            try
            {
                var customerId = _tokenService.GetUserIdFromToken(token);
                // Lấy tất cả đơn hàng của khách hàng
                var orders = await _orderRepository.FindAsync(o => o.CustomerId == customerId);

                // Lọc các đơn hàng đã hoàn thành
                var completedOrders = orders.Where(o => o.StatusId == OrderStatusType.COMPLETED.ToOrderStatusIdString()).ToList();

                // Kiểm tra xem có đơn hàng nào chứa sản phẩm cần tìm không
                foreach (var order in completedOrders)
                {
                    var orderDetails = order.OrderDetails;
                    if (orderDetails != null && orderDetails.Any(od => od.ProductId == productId))
                    {
                        return ServiceResponse<bool>.SuccessResponse(true, "Khách hàng đã mua sản phẩm và đơn hàng đã hoàn thành");
                    }
                }

                return ServiceResponse<bool>.SuccessResponse(false, "Khách hàng chưa mua sản phẩm hoặc đơn hàng chưa hoàn thành");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}