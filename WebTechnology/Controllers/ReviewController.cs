using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Review;
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
        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO review)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.CreateReview(review, token);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("get-list-review")]
        public async Task<IActionResult> GetListReview(string productId)
        {
            var response = await _reviewService.GetProductReviews(productId);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete("delete-review/{id}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _reviewService.DeleteReview(id, token);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
