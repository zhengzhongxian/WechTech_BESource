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
    public class DimensionService : IDimensionService
    {
        private readonly IDimensionRepository _dimensionRepository;
        public DimensionService(IDimensionRepository dimensionRepository)
        {
            _dimensionRepository = dimensionRepository;
        }
        public async Task<ServiceResponse<IEnumerable<Dimension>>> GetDimensionAsync(string productId)
        {
            try
            {
                var dimensions = await _dimensionRepository.GetByPropertyAsync(x => x.Productid, productId);
                if (dimensions == null || !dimensions.Any())
                {
                    return ServiceResponse<IEnumerable<Dimension>>.NotFoundResponse("Không tìm thấy kích thước nào cho sản phẩm này");
                }
                return ServiceResponse<IEnumerable<Dimension>>.SuccessResponse(dimensions, "Lấy danh sách kích thước thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<Dimension>>.ErrorResponse($"Lỗi catch Exception: {ex.Message}");
            }
        }
    }
}
