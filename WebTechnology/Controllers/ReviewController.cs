using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Review;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        /// <summary>
        /// Tạo đánh giá mới cho sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng tạo đánh giá mới cho sản phẩm.
        /// Khách hàng chỉ có thể đánh giá sản phẩm khi đã mua và đơn hàng đã hoàn thành.
        /// </remarks>
        /// <param name="review">Thông tin đánh giá</param>
        /// <returns>Thông tin đánh giá đã tạo</returns>
        [HttpPost("create-review")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO review)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.CreateReview(review, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đánh giá của sản phẩm
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy danh sách tất cả đánh giá của một sản phẩm.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm</param>
        /// <returns>Danh sách đánh giá</returns>
        [HttpGet("get-list-review")]
        public async Task<IActionResult> GetListReview(string productId)
        {
            var response = await _reviewService.GetProductReviews(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách đánh giá của sản phẩm có phân trang
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy danh sách đánh giá của một sản phẩm có phân trang.
        /// </remarks>
        /// <param name="request">Thông tin yêu cầu phân trang</param>
        /// <returns>Danh sách đánh giá có phân trang</returns>
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedReviews([FromQuery] ReviewQueryRequest request)
        {
            var response = await _reviewService.GetPaginatedProductReviews(request);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa đánh giá
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng xóa đánh giá của mình.
        /// </remarks>
        /// <param name="id">ID của đánh giá</param>
        /// <returns>Kết quả xóa đánh giá</returns>
        [HttpDelete("delete-review/{id}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.DeleteReview(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Thêm bình luận vào đánh giá
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng thêm bình luận vào đánh giá.
        /// </remarks>
        /// <param name="comment">Thông tin bình luận</param>
        /// <returns>Thông tin bình luận đã tạo</returns>
        [HttpPost("add-comment")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDTO comment)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.AddComment(comment, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Xóa bình luận
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng xóa bình luận của mình.
        /// </remarks>
        /// <param name="id">ID của bình luận</param>
        /// <returns>Kết quả xóa bình luận</returns>
        [HttpDelete("delete-comment/{id}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.DeleteComment(id, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra xem khách hàng đã mua sản phẩm và đơn hàng đã hoàn thành chưa
        /// </summary>
        /// <remarks>
        /// API này cho phép kiểm tra xem khách hàng đã mua sản phẩm và đơn hàng đã hoàn thành chưa.
        /// </remarks>
        /// <param name="productId">ID của sản phẩm</param>
        /// <returns>Kết quả kiểm tra</returns>
        [HttpGet("check-completed-order/{productId}")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CheckCompletedOrder(string productId)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.HasCompletedOrder(token, productId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
