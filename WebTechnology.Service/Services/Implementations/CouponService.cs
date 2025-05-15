using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Coupons;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class CouponService : ICouponService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CouponService> _logger;
        private readonly IMapper _mapper;

        public CouponService(
            ICustomerRepository customerRepository,
            IVoucherRepository voucherRepository,
            IUnitOfWork unitOfWork,
            ILogger<CouponService> logger,
            IMapper mapper)
        {
            _customerRepository = customerRepository;
            _voucherRepository = voucherRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách các voucher có thể đổi bằng điểm coupon
        /// </summary>
        public async Task<ServiceResponse<List<CouponVoucherDTO>>> GetAvailableVouchersAsync()
        {
            try
            {
                // Lấy danh sách voucher gốc có điểm coupon (Point > 0) và còn hoạt động (IsActive = true)
                var vouchers = await _voucherRepository.FindAsync(v =>
                    v.IsRoot == true &&
                    v.Point.HasValue &&
                    v.Point > 0 &&
                    v.IsActive == true &&
                    v.EndDate > DateTime.UtcNow);

                // Lọc các voucher đã đạt giới hạn sử dụng
                vouchers = vouchers.Where(v => !v.UsageLimit.HasValue || v.UsedCount < v.UsageLimit).ToList();

                // Chuyển đổi sang DTO
                var voucherDTOs = new List<CouponVoucherDTO>();
                foreach (var voucher in vouchers)
                {
                    voucherDTOs.Add(new CouponVoucherDTO
                    {
                        VoucherId = voucher.Voucherid,
                        Code = voucher.Code,
                        DiscountValue = voucher.DiscountValue ?? 0,
                        DiscountType = voucher.DiscountType ?? DiscountType.Percentage,
                        StartDate = voucher.StartDate ?? DateTime.UtcNow,
                        EndDate = voucher.EndDate ?? DateTime.UtcNow.AddMonths(1),
                        MinOrder = voucher.MinOrder,
                        MaxDiscount = voucher.MaxDiscount,
                        PointsRequired = voucher.Point ?? 0,
                        Description = GetVoucherDescription(voucher)
                    });
                }

                return ServiceResponse<List<CouponVoucherDTO>>.SuccessResponse(voucherDTOs, "Lấy danh sách voucher thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher có thể đổi");
                return ServiceResponse<List<CouponVoucherDTO>>.ErrorResponse($"Lỗi khi lấy danh sách voucher: {ex.Message}");
            }
        }

        /// <summary>
        /// Đổi điểm coupon lấy voucher
        /// </summary>
        public async Task<ServiceResponse<RedeemCouponResponseDTO>> RedeemCouponAsync(RedeemCouponDTO redeemDto, string customerId)
        {
            try
            {
                // Bắt đầu transaction
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Lấy thông tin khách hàng
                    var customer = await _customerRepository.GetByIdAsync(customerId);
                    if (customer == null)
                        return ServiceResponse<RedeemCouponResponseDTO>.NotFoundResponse("Không tìm thấy thông tin khách hàng");

                    // Lấy thông tin voucher
                    var selectedVoucher = await _voucherRepository.GetByIdAsync(redeemDto.VoucherId);
                    if (selectedVoucher == null)
                        return ServiceResponse<RedeemCouponResponseDTO>.NotFoundResponse("Không tìm thấy voucher");

                    // Kiểm tra xem voucher có point không
                    if (!selectedVoucher.Point.HasValue || selectedVoucher.Point <= 0)
                        return ServiceResponse<RedeemCouponResponseDTO>.FailResponse("Voucher này không thể đổi bằng điểm coupon");

                    // Kiểm tra số điểm coupon
                    if (customer.Coupoun == null || customer.Coupoun < selectedVoucher.Point)
                        return ServiceResponse<RedeemCouponResponseDTO>.FailResponse("Số điểm coupon không đủ để đổi voucher này");

                    // Kiểm tra xem voucher đã đạt giới hạn sử dụng chưa
                    if (selectedVoucher.UsageLimit.HasValue && selectedVoucher.UsedCount >= selectedVoucher.UsageLimit)
                        return ServiceResponse<RedeemCouponResponseDTO>.FailResponse("Voucher này đã đạt giới hạn sử dụng");

                    // Tạo mã voucher duy nhất
                    string uniqueCode = selectedVoucher.Code + "-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                    // Tạo voucher mới
                    var newVoucher = new Voucher
                    {
                        Voucherid = Guid.NewGuid().ToString(),
                        Code = uniqueCode,
                        DiscountValue = selectedVoucher.DiscountValue,
                        DiscountType = selectedVoucher.DiscountType,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        UsageLimit = 1, // Voucher chỉ sử dụng được 1 lần
                        UsedCount = 0,
                        IsRoot = false,
                        MinOrder = selectedVoucher.MinOrder,
                        MaxDiscount = selectedVoucher.MaxDiscount,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Point = selectedVoucher.Point, // Sử dụng cột point từ voucher gốc
                        Metadata = $"Redeemed by {customerId} with {selectedVoucher.Point} points"
                    };

                    // Lưu voucher mới
                    await _voucherRepository.AddAsync(newVoucher);

                    // Trừ điểm coupon
                    customer.Coupoun -= selectedVoucher.Point.Value;
                    await _customerRepository.UpdateAsync(customer);

                    // Tăng UsedCount của voucher gốc
                    selectedVoucher.UsedCount += 1;
                    await _voucherRepository.UpdateAsync(selectedVoucher);

                    // Commit transaction
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    // Tạo response
                    var response = new RedeemCouponResponseDTO
                    {
                        VoucherId = newVoucher.Voucherid,
                        VoucherCode = newVoucher.Code,
                        PointsUsed = selectedVoucher.Point.Value,
                        RemainingPoints = customer.Coupoun ?? 0,
                        VoucherInfo = GetVoucherDescription(selectedVoucher),
                        ExpiryDate = newVoucher.EndDate ?? DateTime.UtcNow.AddMonths(1)
                    };

                    return ServiceResponse<RedeemCouponResponseDTO>.SuccessResponse(response, "Đổi điểm coupon thành công");
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await _unitOfWork.RollbackAsync();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đổi điểm coupon");
                return ServiceResponse<RedeemCouponResponseDTO>.ErrorResponse($"Lỗi khi đổi điểm coupon: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy lịch sử đổi điểm coupon của khách hàng
        /// </summary>
        public async Task<ServiceResponse<List<RedeemCouponResponseDTO>>> GetRedemptionHistoryAsync(string customerId)
        {
            try
            {
                // Lấy danh sách voucher đã đổi của khách hàng
                // Trong thực tế, bạn có thể lưu lịch sử đổi điểm trong database
                // Ở đây, tôi sẽ trả về một danh sách trống để minh họa
                var redemptionHistory = new List<RedeemCouponResponseDTO>();

                // Lấy danh sách voucher có metadata chứa customerId
                var vouchers = await _voucherRepository.FindAsync(v => v.Metadata != null && v.Metadata.Contains(customerId));
                foreach (var voucher in vouchers)
                {
                    // Phân tích metadata để lấy thông tin về số điểm đã sử dụng
                    var metadata = voucher.Metadata;
                    int pointsUsed = 0;
                    if (metadata.Contains("points"))
                    {
                        var parts = metadata.Split(' ');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i] == "with" && i + 1 < parts.Length)
                            {
                                int.TryParse(parts[i + 1], out pointsUsed);
                                break;
                            }
                        }
                    }

                    // Tạo thông tin về voucher đã đổi
                    var redemption = new RedeemCouponResponseDTO
                    {
                        VoucherId = voucher.Voucherid,
                        VoucherCode = voucher.Code,
                        PointsUsed = pointsUsed,
                        RemainingPoints = 0, // Không có thông tin về số điểm còn lại tại thời điểm đổi
                        VoucherInfo = GetVoucherDescription(voucher),
                        ExpiryDate = voucher.EndDate ?? DateTime.UtcNow
                    };

                    redemptionHistory.Add(redemption);
                }

                return ServiceResponse<List<RedeemCouponResponseDTO>>.SuccessResponse(redemptionHistory, "Lấy lịch sử đổi điểm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy lịch sử đổi điểm");
                return ServiceResponse<List<RedeemCouponResponseDTO>>.ErrorResponse($"Lỗi khi lấy lịch sử đổi điểm: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy số điểm coupon hiện tại của khách hàng
        /// </summary>
        public async Task<ServiceResponse<int>> GetCurrentPointsAsync(string customerId)
        {
            try
            {
                // Lấy thông tin khách hàng
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    return ServiceResponse<int>.NotFoundResponse("Không tìm thấy thông tin khách hàng");

                // Trả về số điểm coupon hiện tại
                return ServiceResponse<int>.SuccessResponse(customer.Coupoun ?? 0, "Lấy số điểm coupon thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy số điểm coupon");
                return ServiceResponse<int>.ErrorResponse($"Lỗi khi lấy số điểm coupon: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo mô tả cho voucher
        /// </summary>
        private string GetVoucherDescription(Voucher voucher)
        {
            string description = "";

            if (voucher.DiscountType == DiscountType.Percentage)
            {
                description = $"Giảm {voucher.DiscountValue}% ";
                if (voucher.MaxDiscount.HasValue)
                {
                    description += $"tối đa {voucher.MaxDiscount.Value:N0}đ ";
                }
            }
            else if (voucher.DiscountType == DiscountType.FixedAmount)
            {
                description = $"Giảm {voucher.DiscountValue:N0}đ ";
            }

            if (voucher.MinOrder.HasValue)
            {
                description += $"cho đơn hàng từ {voucher.MinOrder.Value:N0}đ";
            }
            else
            {
                description += "cho tất cả đơn hàng";
            }

            return description;
        }
    }
}
