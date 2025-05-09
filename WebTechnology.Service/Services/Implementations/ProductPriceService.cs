using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.ProductPrices;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductPriceService> _logger;

        public ProductPriceService(
            IProductPriceRepository productPriceRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProductPriceService> logger)
        {
            _productPriceRepository = productPriceRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<ProductPrice>>> GetProductPricesAsync(string productId)
        {
            try
            {
                var productPrices = await _productPriceRepository.GetByPropertyAsync(x => x.Productid, productId);
                if (productPrices == null || !productPrices.Any())
                {
                    return ServiceResponse<IEnumerable<ProductPrice>>.NotFoundResponse("Không tìm thấy giá nào cho sản phẩm này");
                }
                return ServiceResponse<IEnumerable<ProductPrice>>.SuccessResponse(productPrices, "Lấy danh sách giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách giá sản phẩm: {Message}", ex.Message);
                return ServiceResponse<IEnumerable<ProductPrice>>.ErrorResponse($"Lỗi khi lấy danh sách giá sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ProductPrice>> GetProductPriceByIdAsync(string priceId)
        {
            try
            {
                var productPrice = await _productPriceRepository.GetByIdAsync(priceId);
                if (productPrice == null)
                {
                    return ServiceResponse<ProductPrice>.NotFoundResponse($"Không tìm thấy giá với ID {priceId}");
                }
                return ServiceResponse<ProductPrice>.SuccessResponse(productPrice, "Lấy thông tin giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin giá sản phẩm: {Message}", ex.Message);
                return ServiceResponse<ProductPrice>.ErrorResponse($"Lỗi khi lấy thông tin giá sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ProductPrice>> CreateProductPriceAsync(ProductPriceCreateDTO createDto)
        {
            try
            {
                // Validate ProductId if provided
                if (!string.IsNullOrEmpty(createDto.ProductId))
                {
                    var productExists = await _productRepository.ExistsAsync(p => p.Productid == createDto.ProductId);
                    if (!productExists)
                    {
                        return ServiceResponse<ProductPrice>.ErrorResponse(
                            $"Sản phẩm với ID '{createDto.ProductId}' không tồn tại. Vui lòng sử dụng ID hợp lệ.");
                    }
                }

                // Create new product price
                var productPrice = new ProductPrice
                {
                    Ppsid = Guid.NewGuid().ToString(),
                    Productid = createDto.ProductId,
                    Price = createDto.Price,
                    IsDefault = createDto.IsDefault,
                    IsActive = createDto.IsActive
                };

                // If this price is set as default, update other prices for this product
                if (createDto.IsDefault && !string.IsNullOrEmpty(createDto.ProductId))
                {
                    await UpdateDefaultPriceStatusAsync(createDto.ProductId, productPrice.Ppsid);
                }

                await _productPriceRepository.AddAsync(productPrice);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<ProductPrice>.SuccessResponse(productPrice, "Tạo giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo giá sản phẩm: {Message}", ex.Message);
                return ServiceResponse<ProductPrice>.ErrorResponse($"Lỗi khi tạo giá sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<ProductPrice>> UpdateProductPriceAsync(string priceId, JsonPatchDocument<ProductPrice> patchDoc)
        {
            try
            {
                var productPrice = await _productPriceRepository.GetByIdAsync(priceId);
                if (productPrice == null)
                {
                    return ServiceResponse<ProductPrice>.NotFoundResponse($"Không tìm thấy giá với ID {priceId}");
                }

                // Apply patch operations
                patchDoc.ApplyTo(productPrice);

                // If this price is set as default, update other prices for this product
                if (productPrice.IsDefault == true && !string.IsNullOrEmpty(productPrice.Productid))
                {
                    await UpdateDefaultPriceStatusAsync(productPrice.Productid, productPrice.Ppsid);
                }

                await _productPriceRepository.UpdateAsync(productPrice);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<ProductPrice>.SuccessResponse(productPrice, "Cập nhật giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giá sản phẩm: {Message}", ex.Message);
                return ServiceResponse<ProductPrice>.ErrorResponse($"Lỗi khi cập nhật giá sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductPriceAsync(string priceId)
        {
            try
            {
                var productPrice = await _productPriceRepository.GetByIdAsync(priceId);
                if (productPrice == null)
                {
                    return ServiceResponse<bool>.NotFoundResponse($"Không tìm thấy giá với ID {priceId}");
                }

                await _productPriceRepository.DeleteAsync(productPrice);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Xóa giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giá sản phẩm: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi xóa giá sản phẩm: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> SetDefaultPriceAsync(string priceId, bool isDefault)
        {
            try
            {
                var productPrice = await _productPriceRepository.GetByIdAsync(priceId);
                if (productPrice == null)
                {
                    return ServiceResponse<bool>.NotFoundResponse($"Không tìm thấy giá với ID {priceId}");
                }

                // Only update if the value is changing
                if (productPrice.IsDefault != isDefault)
                {
                    productPrice.IsDefault = isDefault;

                    // If setting as default, update other prices
                    if (isDefault && !string.IsNullOrEmpty(productPrice.Productid))
                    {
                        await UpdateDefaultPriceStatusAsync(productPrice.Productid, productPrice.Ppsid);
                    }

                    await _productPriceRepository.UpdateAsync(productPrice);
                    await _unitOfWork.SaveChangesAsync();
                }

                return ServiceResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái giá mặc định thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái giá mặc định: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi cập nhật trạng thái giá mặc định: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> SetPriceStatusAsync(string priceId, bool isActive)
        {
            try
            {
                var productPrice = await _productPriceRepository.GetByIdAsync(priceId);
                if (productPrice == null)
                {
                    return ServiceResponse<bool>.NotFoundResponse($"Không tìm thấy giá với ID {priceId}");
                }

                productPrice.IsActive = isActive;
                await _productPriceRepository.UpdateAsync(productPrice);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái giá thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái giá: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi cập nhật trạng thái giá: {ex.Message}");
            }
        }

        // Helper method to update default price status
        private async Task UpdateDefaultPriceStatusAsync(string productId, string currentPriceId)
        {
            var otherPrices = await _productPriceRepository.GetByPropertyAsync(
                x => x.Productid,
                productId,
                p => p.Ppsid != currentPriceId && p.IsDefault == true);

            foreach (var price in otherPrices)
            {
                price.IsDefault = false;
                await _productPriceRepository.UpdateAsync(price);
            }
        }
    }
}
