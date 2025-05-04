using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductPriceService(IProductPriceRepository productPriceRepository, IUnitOfWork unitOfWork)
        {
            _productPriceRepository = productPriceRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse<IEnumerable<ProductPrice>>> GetProductPricesAsync(string productId)
        {
            var response = new ServiceResponse<IEnumerable<ProductPrice>>();
            try
            {
                var productPrices = await _productPriceRepository.GetByPropertyAsync(x => x.Productid, productId);
                if (productPrices == null || !productPrices.Any())
                {
                    return ServiceResponse<IEnumerable<ProductPrice>>.NotFoundResponse("Không tìm thấy sản phẩm nào");
                }
                return ServiceResponse<IEnumerable<ProductPrice>>.SuccessResponse(productPrices, "Lấy danh sách giá sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<ProductPrice>>.ErrorResponse($"Lỗi nhé: {ex.Message}");
            }
        }
    }
}
