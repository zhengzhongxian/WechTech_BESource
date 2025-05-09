using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.ProductPrices;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IProductPriceService
    {
        Task<ServiceResponse<IEnumerable<ProductPrice>>> GetProductPricesAsync(string productId);
        Task<ServiceResponse<ProductPrice>> GetProductPriceByIdAsync(string priceId);
        Task<ServiceResponse<ProductPrice>> CreateProductPriceAsync(ProductPriceCreateDTO createDto);
        Task<ServiceResponse<ProductPrice>> UpdateProductPriceAsync(string priceId, JsonPatchDocument<ProductPrice> patchDoc);
        Task<ServiceResponse<bool>> DeleteProductPriceAsync(string priceId);
        Task<ServiceResponse<bool>> SetDefaultPriceAsync(string priceId, bool isDefault);
        Task<ServiceResponse<bool>> SetPriceStatusAsync(string priceId, bool isActive);
    }
}
