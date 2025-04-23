using WebTechnology.Repository.DTOs.Review;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ServiceResponse<ReviewDTO>> CreateReview(CreateReviewDTO review, string token);
        Task<ServiceResponse<List<ReviewDTO>>> GetProductReviews(string productId);
        Task<ServiceResponse<string>> DeleteReview(string reviewId, string token);
    }
} 