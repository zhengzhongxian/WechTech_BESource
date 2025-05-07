using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Cart;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Repository.Repositories.Implementations;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITokenService _tokenService;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IImageRepository _imageRepository;
        public CartItemService(ICartItemRepository cartItemRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ITokenService tokenService,
            IProductPriceRepository productPriceRepository,
            IImageRepository imageRepository)
        {
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _tokenService = tokenService;
            _productPriceRepository = productPriceRepository;
            _imageRepository = imageRepository;
        }

        public async Task<ServiceResponse<string>> AddToCart(CreateCartItemDTO cartItem, string token)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<string>.FailResponse("Token đã hết hạn");
                }

                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                {
                    return ServiceResponse<string>.FailResponse("Không tìm thấy thông tin người dùng");
                }
                var cart = await _cartRepository.GetByIdAsync(userId);
                if (cart == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Giỏ hàng không tồn tại");
                }
                var product = await _productRepository.ExistsAsync(x => x.Productid ==  cartItem.ProductId);
                if (!product)
                {
                    return ServiceResponse<string>.NotFoundResponse("Sản phẩm không tồn tại");
                }
                var newCartItem = _mapper.Map<CartItem>(cartItem);
                newCartItem.CartId = userId;
                cart.CartItems.Add(newCartItem);
                cart.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Thêm vào giỏ hàng thành công");

            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> DeleteCartItem(string id)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                var cartItem = _cartItemRepository.GetByIdAsync(id);
                if (cartItem == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy sản phẩm trong giỏ hàng");
                }
                await _cartItemRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Xóa sản phẩm trong giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<CartItemDTO>>> GetListCartItem(string token)
        {
            var serviceResponse = new ServiceResponse<List<CartItemDTO>>();
            try
            {
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                {
                    return ServiceResponse<List<CartItemDTO>>.FailResponse("token không hợp lệ");
                }
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<List<CartItemDTO>>.FailResponse("Token đã hết hạn");
                }
                var cart = await _cartRepository.GetByIdAsync(userId);
                if (cart == null)
                {
                    return ServiceResponse<List<CartItemDTO>>.NotFoundResponse("Giỏ hàng không tồn tại");
                }
                var cartItems = await _cartItemRepository.GetByPropertyAsync(x => x.CartId, "108e04d8-ba04-49a1-86bb-e775c726b382");
                var cartItemsDto = _mapper.Map<List<CartItemDTO>>(cartItems);
                foreach (var item in cartItemsDto)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    var getProductPrice = await _productPriceRepository.GetProductPriceAsync(item.ProductId);
                    var img = await _imageRepository.GetImageByOrder("1", product.Productid);
                    if (product != null)
                    {
                        item.GetProductToCart = new GetProductToCart
                        {
                            ProductName = product.ProductName ?? "",
                            ProductPriceIsActive = getProductPrice?.PriceIsActive ?? 0,
                            ProductPriceIsDefault = getProductPrice?.PriceIsDefault ?? 0,
                            ProductImgData = img?.ImageData ?? "",
                            TotalPrice = (getProductPrice?.PriceIsActive ?? 0) * item.Quantity,
                        };
                    }
                }
                return ServiceResponse<List<CartItemDTO>>.SuccessResponse(cartItemsDto, "Lấy danh sách sản phẩm trong giỏ hàng thành công nhé các FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<CartItemDTO>>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateCartItem(string id, JsonPatchDocument<CartItem> patchDoc)
        {
            var serviceResponse = new ServiceResponse<string>();
            try
            {
                var cartItem = await _cartItemRepository.GetByIdAsync(id);
                if (cartItem == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                patchDoc.ApplyTo(cartItem);
                
                var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
                if (cart != null)
                {
                    cart.UpdatedAt = DateTime.Now;
                }

                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Cập nhật sản phẩm trong giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }
    }
}
