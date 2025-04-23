using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Review;
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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<ReviewDTO>>> CreateReview(CreateReviewDTO review)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _reviewService.CreateReview(review, token);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ServiceResponse<List<ReviewDTO>>>> GetProductReviews(string productId)
        {
            var response = await _reviewService.GetProductReviews(productId);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpGet("customer")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<List<ReviewDTO>>>> GetCustomerReviews()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _reviewService.GetCustomerReviews(token);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }

        [HttpDelete("{reviewId}")]
        [Authorize]
        public async Task<ActionResult<ServiceResponse<string>>> DeleteReview(string reviewId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await _reviewService.DeleteReview(reviewId, token);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }
            
            return Ok(response);
        }
    }
} 