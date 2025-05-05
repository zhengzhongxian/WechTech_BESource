using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VoucherService> _logger;


        public VoucherService(
            IVoucherRepository voucherRepository,
            IUnitOfWork unitOfWork,
            ILogger<VoucherService> logger)
        {
            _voucherRepository = voucherRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResponse<Voucher>> GetVoucherAsync(string voucherId)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(voucherId);
                if (voucher == null)
                {
                    return ServiceResponse<Voucher>.NotFoundResponse("Không tìm thấy voucher nào với ID này");
                }
                return ServiceResponse<Voucher>.SuccessResponse(voucher, "Lấy voucher thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Voucher>.ErrorResponse($"Lỗi catch Exception: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Voucher>> CreateVoucherAsync(CreateVoucherDTO createDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(createDto.Code))
                {
                    var voucherExists = await _voucherRepository.ExistsAsync(p => p.Code == createDto.Code);
                    if (!voucherExists)
                    {
                        return ServiceResponse<Voucher>.ErrorResponse(
                            $"Voucher với CODE '{createDto.Code}' không tồn tại. Vui lòng sử dụng code hợp lệ.");
                    }
                }
                var newVoucher = new Voucher
                {
                    Voucherid = Guid.NewGuid().ToString(),
                    Code = createDto.Code,
                    DiscountValue = createDto.DiscountValue,
                    DiscountType = createDto.DiscountType,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    UsageLimit = createDto.UsageLimit,
                    UsedCount = 0,
                    MinOrder = createDto.MinOrder,
                    MaxDiscount = createDto.MaxDiscount,
                    IsActive = createDto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Metadata = createDto.Metadata
                };
                await _voucherRepository.AddAsync(newVoucher);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<Voucher>.SuccessResponse(newVoucher, "Tạo Voucher thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Voucher>.ErrorResponse($"Lỗi catch Exception: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<Voucher>> UpdateVoucherAsync(string voucherId, JsonPatchDocument<Voucher> patchDoc)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(voucherId);

                if (voucher == null)
                {
                    return ServiceResponse<Voucher>.NotFoundResponse("Không tìm thấy voucher nào với ID này");
                }

                patchDoc.ApplyTo(voucher);
                voucher.UpdatedAt = DateTime.UtcNow;
                await _voucherRepository.UpdateAsync(voucher);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Voucher>.SuccessResponse(voucher, "Cập nhật voucher thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật dimension: {Message}", ex.Message);
                return ServiceResponse<Voucher>.ErrorResponse(
                    $"Lỗi khi cập nhật Voucher: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteVoucherAsync(string voucherId)
        {
            try
            {
                var voucher = await _voucherRepository.GetByIdAsync(voucherId);

                if (voucher == null)
                {
                    return ServiceResponse<bool>.NotFoundResponse("Không tìm thấy voucher nào với ID này");
                }

                await _voucherRepository.DeleteAsync(voucher);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Xóa voucher thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa voucher theo code: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse(
                    $"Lỗi khi xóa Voucher: {ex.Message}");
            }
        }
    }

}