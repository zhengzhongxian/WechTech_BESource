using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResponse<PaginatedResult<GetProductDTO>>> GetProductsWithDetailsAsync(ProductQueryRequest request);
        Task<ServiceResponse<string>> CreateProductTrendsAsync(CreateProductTrendsDTO createDto);
        Task<ServiceResponse<string>> PatchProductTrendsAsync(string id, JsonPatchDocument<ProductTrend> patchDoc);
        Task<ServiceResponse<string>> DeleteProductTrendsAsync(string id);
        Task<ServiceResponse<List<GetListProductTrends>>> GetListTrendsByProductId(string productId);
        Task<ServiceResponse<Product>> CreateProductAsync(CreateProductDTO createDto);
        Task<ServiceResponse<IEnumerable<Product>>> GetProductsAsync();
        Task<ServiceResponse<Product>> GetProductByIdAsync(string id);
        Task<ServiceResponse<Product>> PatchProductAsync(string id, JsonPatchDocument<Product> patchDoc);
        Task<ServiceResponse<bool>> DeleteProductAsync(string id);
    }
}
