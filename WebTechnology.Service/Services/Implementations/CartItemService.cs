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
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Repository.Models.Pagination;
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

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
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

                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Sản phẩm không tồn tại");
                }

                // Kiểm tra sản phẩm đã hết hàng chưa
                if (product.Stockquantity <= 0)
                {
                    return ServiceResponse<string>.FailResponse(
                        "Sản phẩm đã hết hàng",
                        HttpStatusCode.BadRequest);
                }

                // Kiểm tra số lượng tồn kho
                if (product.Stockquantity < cartItem.Quantity)
                {
                    return ServiceResponse<string>.FailResponse(
                        $"Số lượng yêu cầu ({cartItem.Quantity}) vượt quá số lượng tồn kho ({product.Stockquantity})",
                        HttpStatusCode.BadRequest);
                }

                // Kiểm tra sản phẩm đã có trong giỏ hàng chưa bằng cách truy vấn trực tiếp
                var cartItems = await _cartItemRepository.GetByPropertyAsync(
                    ci => ci.CartId,
                    userId,
                    ci => ci.Productid == cartItem.ProductId
                );
                var existingCartItem = cartItems.FirstOrDefault();

                if (existingCartItem != null)
                {
                    // Nếu đã có, cập nhật số lượng
                    int newQuantity = existingCartItem.Quantity.GetValueOrDefault() + cartItem.Quantity;

                    // Kiểm tra lại số lượng tồn kho sau khi cộng dồn
                    if (product.Stockquantity < newQuantity)
                    {
                        return ServiceResponse<string>.FailResponse(
                            $"Tổng số lượng ({newQuantity}) vượt quá số lượng tồn kho ({product.Stockquantity})",
                            HttpStatusCode.BadRequest);
                    }

                    existingCartItem.Quantity = newQuantity;
                    existingCartItem.UpdatedAt = DateTime.Now; // Cập nhật thời gian cập nhật
                    await _cartItemRepository.UpdateAsync(existingCartItem);
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    var newCartItem = _mapper.Map<CartItem>(cartItem);
                    newCartItem.CartId = userId;
                    newCartItem.Id = Guid.NewGuid().ToString();
                    newCartItem.CreatedAt = DateTime.Now; // Khởi tạo thời gian tạo
                    newCartItem.UpdatedAt = DateTime.Now; // Khởi tạo thời gian cập nhật
                    await _cartItemRepository.AddAsync(newCartItem);
                }

                cart.UpdatedAt = DateTime.Now;
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Thêm vào giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        public async Task<ServiceResponse<string>> DeleteCartItem(string id, string token)
        {
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

                // Kiểm tra sản phẩm trong giỏ hàng
                var cartItem = await _cartItemRepository.GetByIdAsync(id);
                if (cartItem == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                // Kiểm tra giỏ hàng thuộc về người dùng hiện tại
                var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.Cartid != userId)
                {
                    return ServiceResponse<string>.FailResponse("Bạn không có quyền xóa sản phẩm này", HttpStatusCode.Forbidden);
                }

                // Xóa sản phẩm khỏi giỏ hàng
                await _cartItemRepository.DeleteAsync(id);

                // Cập nhật thời gian cập nhật giỏ hàng
                cart.UpdatedAt = DateTime.Now;

                // Lưu thay đổi
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Xóa sản phẩm trong giỏ hàng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm trong giỏ hàng có phân trang
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<CartItemDTO>>> GetListCartItem(string token, CartItemQueryRequest request)
        {
            try
            {
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                {
                    return ServiceResponse<PaginatedResult<CartItemDTO>>.FailResponse("Token không hợp lệ");
                }

                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<PaginatedResult<CartItemDTO>>.FailResponse("Token đã hết hạn");
                }

                var cart = await _cartRepository.GetByIdAsync(userId);
                if (cart == null)
                {
                    return ServiceResponse<PaginatedResult<CartItemDTO>>.NotFoundResponse("Giỏ hàng không tồn tại");
                }

                // Lấy danh sách sản phẩm trong giỏ hàng
                var cartItems = await _cartItemRepository.GetByPropertyAsync<string>(x => x.CartId, cart.Cartid);
                var cartItemsDto = _mapper.Map<List<CartItemDTO>>(cartItems);

                // Xử lý từng sản phẩm trong giỏ hàng
                foreach (var item in cartItemsDto)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);

                    // Kiểm tra sản phẩm còn tồn tại không
                    if (product == null || product.IsDeleted == true || product.IsActive == false)
                    {
                        item.IsProductExists = false;
                        continue;
                    }

                    // Kiểm tra số lượng tồn kho
                    item.AvailableStock = product.Stockquantity;

                    // Kiểm tra sản phẩm đã hết hàng chưa
                    if (product.Stockquantity <= 0)
                    {
                        item.IsOutOfStock = true;
                    }

                    // Kiểm tra số lượng có vượt quá tồn kho không
                    if (product.Stockquantity < item.Quantity)
                    {
                        item.IsStockAvailable = false;
                    }

                    // Lấy thông tin giá sản phẩm
                    var getProductPrice = await _productPriceRepository.GetProductPriceAsync(item.ProductId);

                    // Lấy hình ảnh sản phẩm
                    string? productIdForImage = product.Productid;
                    ImageDTO? img = null;
                    if (!string.IsNullOrEmpty(productIdForImage))
                    {
                        img = await _imageRepository.GetImageByOrder("1", productIdForImage);
                    }

                    // Tạo thông tin sản phẩm hiển thị trong giỏ hàng
                    if (getProductPrice != null)
                    {
                        item.GetProductToCart = new GetProductToCart
                        {
                            ProductName = product.ProductName ?? "",
                            ProductPriceIsActive = getProductPrice.PriceIsActive,
                            ProductPriceIsDefault = getProductPrice.PriceIsDefault,
                            ProductImgData = img?.ImageData ?? "",
                            TotalPrice = getProductPrice.PriceIsActive * item.Quantity,
                        };
                    }
                }

                // Lọc theo từ khóa tìm kiếm nếu có
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    cartItemsDto = cartItemsDto
                        .Where(item => item.GetProductToCart?.ProductName?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();
                }

                // Sắp xếp
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    switch (request.SortBy.ToLower())
                    {
                        case "productname":
                            cartItemsDto = request.SortAscending
                                ? cartItemsDto.OrderBy(item => item.GetProductToCart?.ProductName).ToList()
                                : cartItemsDto.OrderByDescending(item => item.GetProductToCart?.ProductName).ToList();
                            break;
                        case "price":
                            cartItemsDto = request.SortAscending
                                ? cartItemsDto.OrderBy(item => item.GetProductToCart?.ProductPriceIsActive).ToList()
                                : cartItemsDto.OrderByDescending(item => item.GetProductToCart?.ProductPriceIsActive).ToList();
                            break;
                        case "quantity":
                            cartItemsDto = request.SortAscending
                                ? cartItemsDto.OrderBy(item => item.Quantity).ToList()
                                : cartItemsDto.OrderByDescending(item => item.Quantity).ToList();
                            break;
                        case "totalprice":
                            cartItemsDto = request.SortAscending
                                ? cartItemsDto.OrderBy(item => item.GetProductToCart?.TotalPrice).ToList()
                                : cartItemsDto.OrderByDescending(item => item.GetProductToCart?.TotalPrice).ToList();
                            break;
                        case "updatedat":
                            cartItemsDto = request.SortAscending
                                ? cartItemsDto.OrderBy(item => item.UpdatedAt).ToList()
                                : cartItemsDto.OrderByDescending(item => item.UpdatedAt).ToList();
                            break;
                        default:
                            // Mặc định sắp xếp theo ngày cập nhật (giảm dần - mới nhất lên đầu)
                            cartItemsDto = cartItemsDto.OrderByDescending(item => item.UpdatedAt).ToList();
                            break;
                    }
                }
                else
                {
                    // Nếu không có SortBy, mặc định sắp xếp theo ngày cập nhật (giảm dần - mới nhất lên đầu)
                    cartItemsDto = cartItemsDto.OrderByDescending(item => item.UpdatedAt).ToList();
                }

                // Phân trang
                var totalCount = cartItemsDto.Count;
                var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

                var paginatedItems = cartItemsDto
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var metadata = new PaginationMetadata(
                    request.PageNumber,
                    request.PageSize,
                    totalCount
                );

                var result = new PaginatedResult<CartItemDTO>(paginatedItems, metadata);

                return ServiceResponse<PaginatedResult<CartItemDTO>>.SuccessResponse(
                    result,
                    "Lấy danh sách sản phẩm trong giỏ hàng thành công nhé các FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<CartItemDTO>>.ErrorResponse(
                    $"Có lỗi phía server nhé FE : {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật sản phẩm trong giỏ hàng
        /// </summary>
        public async Task<ServiceResponse<string>> UpdateCartItem(string id, JsonPatchDocument<CartItem> patchDoc, string token)
        {
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

                // Kiểm tra sản phẩm trong giỏ hàng
                var cartItem = await _cartItemRepository.GetByIdAsync(id);
                if (cartItem == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy sản phẩm trong giỏ hàng");
                }

                // Kiểm tra giỏ hàng thuộc về người dùng hiện tại
                var cart = await _cartRepository.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.Cartid != userId)
                {
                    return ServiceResponse<string>.FailResponse("Bạn không có quyền cập nhật sản phẩm này", HttpStatusCode.Forbidden);
                }

                // Lưu lại số lượng cũ để kiểm tra
                int? oldQuantity = cartItem.Quantity;

                // Áp dụng các thay đổi
                patchDoc.ApplyTo(cartItem);

                // Cập nhật thời gian cập nhật
                cartItem.UpdatedAt = DateTime.Now;

                // Kiểm tra số lượng mới
                if (cartItem.Quantity <= 0)
                {
                    return ServiceResponse<string>.FailResponse("Số lượng phải lớn hơn 0", HttpStatusCode.BadRequest);
                }

                // Kiểm tra số lượng tồn kho
                var product = await _productRepository.GetByIdAsync(cartItem.Productid);
                if (product == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Sản phẩm không tồn tại");
                }

                // Kiểm tra sản phẩm đã hết hàng chưa
                if (product.Stockquantity <= 0)
                {
                    return ServiceResponse<string>.FailResponse(
                        "Sản phẩm đã hết hàng",
                        HttpStatusCode.BadRequest);
                }

                // Kiểm tra số lượng tồn kho
                if (product.Stockquantity < cartItem.Quantity)
                {
                    return ServiceResponse<string>.FailResponse(
                        $"Số lượng yêu cầu ({cartItem.Quantity}) vượt quá số lượng tồn kho ({product.Stockquantity})",
                        HttpStatusCode.BadRequest);
                }

                // Cập nhật thời gian cập nhật giỏ hàng
                cart.UpdatedAt = DateTime.Now;

                // Lưu thay đổi
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
