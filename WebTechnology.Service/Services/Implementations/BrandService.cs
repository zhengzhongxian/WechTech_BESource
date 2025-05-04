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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<ServiceResponse<IEnumerable<Brand>>> GetBrandsAsync()
        {
            try
            {
                var brands = await _brandRepository.GetAllAsync();
                return ServiceResponse<IEnumerable<Brand>>.SuccessResponse(brands, "Lấy danh sách thương hiệu thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<Brand>>.ErrorResponse($"Lỗi catch Exception: {ex.Message}");
            }
        }
    }
}
