using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Cart;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<ServiceResponse<string>> AddToCart(CreateCartItemDTO cartItem, string token);
        Task<ServiceResponse<List<CartItemDTO>>> GetListCartItem(string token);
        Task<ServiceResponse<string>> UpdateCartItem(string id, JsonPatchDocument<CartItem> patchDoc);
        Task<ServiceResponse<string>> DeleteCartItem(string id);

    }
}
