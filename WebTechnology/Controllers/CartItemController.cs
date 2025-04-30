using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Cart;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartItemDTO cartItem)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.AddToCart(cartItem, token);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("get-list-cart-item")]
        public async Task<IActionResult> GetListCartItem()
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.GetListCartItem(token);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPatch("update-cart-item/{id}")]
        public async Task<IActionResult> UpdateCartItem(string id, [FromBody] JsonPatchDocument<CartItem> patchDoc)
        {
            var response = await _cartItemService.UpdateCartItem(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete("delete-cart-item/{id}")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            var response = await _cartItemService.DeleteCartItem(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
