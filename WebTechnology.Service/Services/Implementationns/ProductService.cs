using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IDimensionRepository _dimensionRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductStatusRepository _productStatusRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IDimensionRepository dimensionRepository,
            IProductPriceRepository productPriceRepository,
            IProductCategoryRepository productCategoryRepository,
            IImageRepository imageRepository,
            IProductStatusRepository productStatusRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _dimensionRepository = dimensionRepository;
            _productPriceRepository = productPriceRepository;
            _productCategoryRepository = productCategoryRepository;
            _productStatusRepository = productStatusRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResponse<Product>> CreateProductAsync(CreateProductDTO createDto)
        {
            try
            {
                // Validate StatusId if provided
                if (!string.IsNullOrEmpty(createDto.StatusId))
                {
                    // Use a lambda expression instead of passing the string directly
                    var statusExists = await _productStatusRepository.ExistsAsync(s => s.StatusId == createDto.StatusId);
                    if (!statusExists)
                    {
                        return ServiceResponse<Product>.ErrorResponse(
                            $"Trạng thái sản phẩm với ID '{createDto.StatusId}' không tồn tại. Vui lòng sử dụng ID hợp lệ.");
                    }
                }

                // Create new product manually instead of using AutoMapper
                var product = new Product
                {
                    Productid = Guid.NewGuid().ToString(),
                    ProductName = createDto.ProductName,
                    Stockquantity = createDto.StockQuantity,
                    Bar = createDto.Bar,
                    Sku = createDto.Sku,
                    Description = createDto.Description,
                    // Only set Brand if it's not null or empty
                    Brand = string.IsNullOrEmpty(createDto.Brand) ? null : createDto.Brand,
                    Unit = string.IsNullOrEmpty(createDto.Unit) ? null : createDto.Unit,
                    IsActive = createDto.IsActive,
                    IsDeleted = false,
                    StatusId = string.IsNullOrEmpty(createDto.StatusId) ? null : createDto.StatusId,
                    Metadata = createDto.Metadata,
                    CreatedAt = DateTime.UtcNow
                };

                await _productRepository.AddAsync(product);

                // Add dimensions if provided
                if (createDto.WeightValue.HasValue || createDto.HeightValue.HasValue ||
                    createDto.WidthValue.HasValue || createDto.LengthValue.HasValue)
                {
                    var dimension = new Dimension
                    {
                        Dimensionid = Guid.NewGuid().ToString(),
                        Productid = product.Productid,
                        WeightValue = createDto.WeightValue,
                        HeightValue = createDto.HeightValue,
                        WidthValue = createDto.WidthValue,
                        LengthValue = createDto.LengthValue
                    };

                    await _dimensionRepository.AddAsync(dimension);
                }

                // Add price
                var productPrice = new ProductPrice
                {
                    Ppsid = Guid.NewGuid().ToString(),
                    Productid = product.Productid,
                    Price = createDto.Price,
                    IsDefault = true
                };

                await _productPriceRepository.AddAsync(productPrice);

                // Add categories if provided
                if (createDto.CategoryIds != null && createDto.CategoryIds.Any())
                {
                    foreach (var categoryId in createDto.CategoryIds)
                    {
                        var productCategory = new ProductCategory
                        {
                            Id = Guid.NewGuid().ToString(),
                            Productid = product.Productid,
                            Categoryid = categoryId
                        };

                        await _productCategoryRepository.AddAsync(productCategory);
                    }
                }

                // Add images if provided
                if (createDto.ImageData != null && createDto.ImageData.Any())
                {
                    int order = 0;
                    foreach (var imageData in createDto.ImageData)
                    {
                        var image = new Image
                        {
                            Imageid = Guid.NewGuid().ToString(),
                            Productid = product.Productid,
                            ImageData = imageData,
                            Order = order.ToString(),
                            CreatedAt = DateTime.UtcNow
                        };

                        order++;
                        await _imageRepository.AddAsync(image);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Product>.SuccessResponse(product, "Tạo sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm: {Message}", ex.Message);
                return ServiceResponse<Product>.ErrorResponse($"Lỗi khi tạo sản phẩm nhé FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<Product>>> GetProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();

                // Filter out deleted products
                products = products.Where(p => p.IsDeleted != true).ToList();

                return ServiceResponse<IEnumerable<Product>>.SuccessResponse(
                    products, "Lấy danh sách sản phẩm thành công nhé các FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm: {Message}", ex.Message);
                return ServiceResponse<IEnumerable<Product>>.ErrorResponse(
                    $"Lỗi khi lấy dữ liệu nhé các FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Product>> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null || product.IsDeleted == true)
                {
                    return ServiceResponse<Product>.NotFoundResponse("Sản phẩm không tồn tại nhé FE");
                }

                return ServiceResponse<Product>.SuccessResponse(
                    product, "Lấy thông tin sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sản phẩm: {Message}", ex.Message);
                return ServiceResponse<Product>.ErrorResponse(
                    $"Lỗi khi lấy thông tin sản phẩm nhé FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Product>> PatchProductAsync(string id, JsonPatchDocument<Product> patchDoc)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null || product.IsDeleted == true)
                {
                    return ServiceResponse<Product>.NotFoundResponse("Sản phẩm không tồn tại nhé FE");
                }

                patchDoc.ApplyTo(product);
                product.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Product>.SuccessResponse(
                    product, "Cập nhật sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm: {Message}", ex.Message);
                return ServiceResponse<Product>.ErrorResponse(
                    $"Lỗi khi cập nhật sản phẩm nhé FE: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(string id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return ServiceResponse<bool>.NotFoundResponse("Sản phẩm không tồn tại nhé FE");
                }

                // Soft delete
                product.IsDeleted = true;
                product.DeletedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(
                    true, "Xóa sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse(
                    $"Lỗi khi xóa sản phẩm nhé FE: {ex.Message}");
            }
        }
    }
}