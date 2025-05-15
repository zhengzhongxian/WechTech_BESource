using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Extensions;
using Microsoft.Extensions.Logging;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;
using WebTechnology.Repository.Repositories.Implementations;
using WebTechnology.Service.CoreHelpers.Multimedia;

namespace WebTechnology.Service.Services.Implementationns
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductTrendsRepository _productTrendsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IProductStatusRepository _productStatusRepository;
        private readonly IDimensionRepository _dimensionRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductService(IProductRepository productRepository,
            IMapper mapper,
            IProductTrendsRepository productTrendsRepositoy,
            IUnitOfWork unitOfWork,
            IProductCategoryRepository productCategoryRepository,
            IProductPriceRepository productPriceRepository,
            IProductStatusRepository productStatusRepository,
            IDimensionRepository dimensionRepository,
            IImageRepository imageRepository,
            ILogger<ProductService> logger,
            ICloudinaryService cloudinaryService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productTrendsRepository = productTrendsRepositoy;
            _unitOfWork = unitOfWork;
            _productCategoryRepository = productCategoryRepository;
            _productPriceRepository = productPriceRepository;
            _productStatusRepository = productStatusRepository;
            _dimensionRepository = dimensionRepository;
            _imageRepository = imageRepository;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ServiceResponse<string>> CreateProductTrendsAsync(CreateProductTrendsDTO createDto)
        {
            try
            {
                var productTrend = _mapper.Map<ProductTrend>(createDto);
                productTrend.Ptsid = Guid.NewGuid().ToString();
                productTrend.CreatedAt = DateTime.UtcNow;
                await _productTrendsRepository.AddAsync(productTrend);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Thêm xu hướng cho sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi tạo xu hướng cho sản phẩm nhé {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> DeleteProductTrendsAsync(string id)
        {
            try
            {
                var exists = await _productTrendsRepository.GetByIdAsync(id);
                if (exists == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy xu hướng sản phẩm");
                }
                await _productTrendsRepository.DeleteAsync(exists);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Xóa xu hướng sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi xóa xu hướng cho sản phẩm nhé {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<GetListProductTrends>>> GetListTrendsByProductId(string productId)
        {
            try
            {
                var exists = await _productRepository.GetByIdAsync(productId);
                if (exists == null)
                {
                    return ServiceResponse<List<GetListProductTrends>>.NotFoundResponse("Không tìm thấy sản phẩm");
                }
                var result = await _productTrendsRepository.GetByPropertyAsync(x => x.Productid, productId);
                var mappedResult = _mapper.Map<List<GetListProductTrends>>(result);
                return ServiceResponse<List<GetListProductTrends>>.SuccessResponse(mappedResult, "Lấy danh sách xu hướng sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<GetListProductTrends>>.ErrorResponse($"Lỗi khi lấy danh sách xu hướng sản phẩm nhé {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PaginatedResult<GetProductDTO>>> GetProductsWithDetailsAsync(ProductQueryRequest request)
        {
            try
            {
                var query = _productRepository.GetProductAsQueryable()
                    .Where(p => p.IsActive == true && p.IsDeleted != true);

                if (!string.IsNullOrEmpty(request.CategoryId))
                {
                    query = query.Where(p => p.ProductCategories.Any(pc => pc.Categoryid == request.CategoryId));
                }

                if (!string.IsNullOrEmpty(request.TrendId))
                {
                    query = query.Where(p => p.ProductTrends.Any(pt => pt.Trend == request.TrendId));
                }

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(p => p.ProductName.Contains(request.SearchTerm) ||
                                       p.Description.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.SortBy.ToLower() switch
                    {
                        "price-low-high" => query.OrderBy(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price),
                        "price-high-low" => query.OrderByDescending(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price),
                        _ => request.SortAscending
                            ? query.OrderBy(p => p.ProductName)
                            : query.OrderByDescending(p => p.ProductName)
                    };
                }

                var result = await query
                    .AsSplitQuery()
                    .ProjectTo<GetProductDTO>(_mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);

                return ServiceResponse<PaginatedResult<GetProductDTO>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<GetProductDTO>>.ErrorResponse(
                    "An error occurred while retrieving products",
                    HttpStatusCode.InternalServerError,
                    new List<string> { ex.Message });
            }
        }

        public async Task<ServiceResponse<string>> PatchProductTrendsAsync(string id, JsonPatchDocument<ProductTrend> patchDoc)
        {
            try
            {
                var exists = await _productTrendsRepository.GetByIdAsync(id);
                if (exists == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Xu hướng sản không tồn tại nhé FE");
                }
                patchDoc.ApplyTo(exists);
                exists.UpdatedAt = DateTime.UtcNow;
                await _productTrendsRepository.UpdateAsync(exists);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Cập nhật xu hướng sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi cập nhật xu hướng sản phẩm nhé FE: {ex.Message}");
            }
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
                if (createDto.Dimensions != null && createDto.Dimensions.Any())
                {
                    foreach (var dimensionDto in createDto.Dimensions)
                    {
                        var dimension = new Dimension
                        {
                            Dimensionid = Guid.NewGuid().ToString(),
                            Productid = product.Productid,
                            WeightValue = dimensionDto.WeightValue,
                            HeightValue = dimensionDto.HeightValue,
                            WidthValue = dimensionDto.WidthValue,
                            LengthValue = dimensionDto.LengthValue,
                        };

                        await _dimensionRepository.AddAsync(dimension);
                    }
                }

                // Add prices if provided
                if (createDto.ProductPrices != null && createDto.ProductPrices.Any())
                {
                    foreach (var priceDto in createDto.ProductPrices)
                    {
                        var productPrice = new ProductPrice
                        {
                            Ppsid = Guid.NewGuid().ToString(),
                            Productid = product.Productid,
                            Price = priceDto.Price,
                            IsDefault = priceDto.IsDefault,
                            IsActive = priceDto.IsActive,
                        };

                        await _productPriceRepository.AddAsync(productPrice);
                    }
                }

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
                if (createDto.Images != null && createDto.Images.Any())
                {
                    foreach (var imageDto in createDto.Images)
                    {
                        //add to cloudinary
                        var uploadResult = await _cloudinaryService.UploadImageAsync(imageDto.ImageData);
                        if (uploadResult.Error != null)
                        {
                            _logger.LogError($"Lỗi khi upload ảnh: {uploadResult.Error.Message}");
                            continue;
                        }

                        var image = new Image
                        {
                            Imageid = Guid.NewGuid().ToString(),
                            Productid = product.Productid,
                            ImageData = uploadResult.SecureUrl.ToString(),
                            Order = imageDto.Order,
                            CreatedAt = DateTime.UtcNow
                        };

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

        public async Task<ServiceResponse<GetProductDTO>> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _productRepository.GetProductAsQueryable()
                    .Where(p => p.Productid == id && p.IsActive == true && p.IsDeleted != true)
                    .ProjectTo<GetProductDTO>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return ServiceResponse<GetProductDTO>.NotFoundResponse("Sản phẩm không tồn tại nhé FE");
                }

                return ServiceResponse<GetProductDTO>.SuccessResponse(
                    product, "Lấy thông tin sản phẩm thành công nhé FE");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sản phẩm: {Message}", ex.Message);
                return ServiceResponse<GetProductDTO>.ErrorResponse(
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

                return ServiceResponse<Product>.SuccessResponse("Cập nhật sản phẩm thành công nhé FE");
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