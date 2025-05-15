using WebTechnology.Repository.DTOs.Review;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IReviewService
    {
        /// <summary>
        /// Tạo đánh giá mới cho sản phẩm (chỉ khi đã mua sản phẩm và đơn hàng đã hoàn thành)
        /// </summary>
        Task<ServiceResponse<ReviewDTO>> CreateReview(CreateReviewDTO review, string token);

        /// <summary>
        /// Lấy danh sách đánh giá của sản phẩm (không phân trang)
        /// </summary>
        Task<ServiceResponse<List<ReviewDTO>>> GetProductReviews(string productId);

        /// <summary>
        /// Lấy danh sách đánh giá của sản phẩm có phân trang
        /// </summary>
        Task<ServiceResponse<PaginatedResult<ReviewDTO>>> GetPaginatedProductReviews(ReviewQueryRequest request);

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        Task<ServiceResponse<string>> DeleteReview(string reviewId, string token);

        /// <summary>
        /// Thêm bình luận vào đánh giá
        /// </summary>
        Task<ServiceResponse<CommentDTO>> AddComment(CreateCommentDTO comment, string token);

        /// <summary>
        /// Xóa bình luận
        /// </summary>
        Task<ServiceResponse<string>> DeleteComment(string commentId, string token);

        /// <summary>
        /// Kiểm tra xem khách hàng đã mua sản phẩm và đơn hàng đã hoàn thành chưa
        /// </summary>
        Task<ServiceResponse<bool>> HasCompletedOrder(string customerId, string productId);
    }
}