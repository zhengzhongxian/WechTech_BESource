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
    }
}
