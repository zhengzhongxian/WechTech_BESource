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
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IProductTrendsRepository _productTrendsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productRepository,
            IMapper mapper,
            IProductTrendsRepository productTrendsRepositoy,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _productTrendsRepository = productTrendsRepositoy;
            _unitOfWork = unitOfWork;
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
                        "price" => request.SortAscending
                            ? query.OrderBy(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price)
                            : query.OrderByDescending(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price),
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
    }
}
