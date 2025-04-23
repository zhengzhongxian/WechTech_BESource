using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Review;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITokenService _tokenService;

        public ReviewService(
            IReviewRepository reviewRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ITokenService tokenService)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _tokenService = tokenService;
        }

        public async Task<ServiceResponse<ReviewDTO>> CreateReview(CreateReviewDTO review, string token)
        {
            var serviceResponse = new ServiceResponse<ReviewDTO>();
            try
            {
                var customerId = _tokenService.GetUserIdFromToken(token);
                if (customerId == null)
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Không tìm thấy thông tin người dùng");
                }

                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<ReviewDTO>.FailResponse("Token đã hết hạn");
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

                var newReview = new Review
                {
                    Reviewid = Guid.NewGuid().ToString(),
                    Customerid = customerId,
                    Productid = review.ProductId,
                    Rate = review.Rate
                };

                await _reviewRepository.AddAsync(newReview);
                await _unitOfWork.SaveChangesAsync();

                var reviewDto = _mapper.Map<ReviewDTO>(newReview);
                string customerName = customer.Surname + " " + customer.Middlename + " " + customer.Surname;
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
                    string customerName = customer.Surname + " " + customer.Middlename + " " + customer.Surname;
                    reviewDto.CustomerName = customerName ?? "Khách hàng";
                    reviewDtos.Add(reviewDto);
                }

                return ServiceResponse<List<ReviewDTO>>.SuccessResponse(reviewDtos, "Lấy danh sách đánh giá thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<ReviewDTO>>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
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

                await _reviewRepository.DeleteAsync(reviewId);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<string>.SuccessResponse("Xóa đánh giá thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
} 