using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
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

                query = request.SortBy.ToLower() switch
                {
                    "price" => request.SortAscending
                        ? query.OrderBy(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price)
                        : query.OrderByDescending(p => p.ProductPrices.FirstOrDefault(pp => pp.IsActive).Price),
                    _ => request.SortAscending
                        ? query.OrderBy(p => p.ProductName)
                        : query.OrderByDescending(p => p.ProductName)
                };

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
    }
}
