using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class DimensionService : IDimensionService
    {
        private readonly IDimensionRepository _dimensionRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DimensionService> _logger;


        public DimensionService(
            IDimensionRepository dimensionRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ILogger<DimensionService> logger)
        {
            _dimensionRepository = dimensionRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
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

        public async Task<ServiceResponse<Dimension>> CreateDimensionAsync(CreateDimensionDTO createDto)
        {
            try
            {
                // Validate ProductId if provided
                if (!string.IsNullOrEmpty(createDto.ProductId))
                {
                    var productExists = await _productRepository.ExistsAsync(p => p.Productid == createDto.ProductId);
                    if (!productExists)
                    {
                        return ServiceResponse<Dimension>.ErrorResponse(
                            $"Sản phẩm với ID '{createDto.ProductId}' không tồn tại. Vui lòng sử dụng ID hợp lệ.");
                    }
                }

                // Create new dimension
                var dimension = new Dimension
                {
                    Dimensionid = Guid.NewGuid().ToString(),
                    Productid = createDto.ProductId,
                    WeightValue = createDto.WeightValue,
                    HeightValue = createDto.HeightValue,
                    WidthValue = createDto.WidthValue,
                    LengthValue = createDto.LengthValue,
                    Metadata = createDto.Metadata
                };

                await _dimensionRepository.AddAsync(dimension);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Dimension>.SuccessResponse(dimension, "Tạo kích thước thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Dimension>.ErrorResponse($"Lỗi khi tạo kích thước: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Dimension>> UpdateDimensionAsync(string productId, JsonPatchDocument<Dimension> patchDoc)
        {
            try
            {
                var dimensions = await _dimensionRepository.GetByPropertyAsync(x => x.Productid, productId);

                if (dimensions == null || !dimensions.Any())
                {
                    return ServiceResponse<Dimension>.NotFoundResponse("Không tìm thấy kích thước nào cho sản phẩm này");
                }
                
                var dimension = dimensions.First();

                patchDoc.ApplyTo(dimension);

                await _dimensionRepository.UpdateAsync(dimension);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Dimension>.SuccessResponse(dimension, "Cập nhật kích thước thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dimension: {Message}", ex.Message);
                return ServiceResponse<Dimension>.ErrorResponse(
                    $"Lỗi khi cập nhật Dimension: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteDimensionAsync(string productId)
        {
            try
            {
                var dimensions = await _dimensionRepository.GetByPropertyAsync(x => x.Productid, productId);

                if (dimensions == null || !dimensions.Any())
                {
                    return ServiceResponse<bool>.NotFoundResponse("Không tìm thấy kích thước nào cho sản phẩm này");
                }

                foreach (var dimension in dimensions)
                {
                    await _dimensionRepository.DeleteAsync(dimension);
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(
                    true, "Xóa tất cả kích thước của sản phẩm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa dimension theo productId: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse(
                    $"Lỗi khi xóa dimension: {ex.Message}");
            }
        }
    }
}