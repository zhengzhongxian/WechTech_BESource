using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ProductStatusService : IProductStatusService
    {
        private readonly IProductStatusRepository _productStatusRepository;
        public ProductStatusService(IProductStatusRepository productStatusRepository)
        {
            _productStatusRepository = productStatusRepository;
        }
        public async Task<ServiceResponse<IEnumerable<ProductStatus>>> GetAllProductStatus()
        {
            try
            {
                var productStatus = await _productStatusRepository.GetAllAsync();
                return ServiceResponse<IEnumerable<ProductStatus>>.SuccessResponse(productStatus, "Lấy danh sách trạng thái sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<ProductStatus>>.ErrorResponse($"Lỗi nhé: {ex.Message}");
            }
        }
    }
}
