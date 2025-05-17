using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Repository.Models.Pagination;
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
        private readonly ITokenService _tokenService;

        public VoucherService(
            IVoucherRepository voucherRepository,
            IUnitOfWork unitOfWork,
            ILogger<VoucherService> logger,
            ITokenService tokenService)
        {
            _voucherRepository = voucherRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
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
                // Tạo voucher mới mà không cần kiểm tra code
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
                    Metadata = createDto.Metadata,
                    IsRoot = createDto.IsRoot,
                    Point = createDto.Point,
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

                // Đánh dấu voucher là đã xóa thay vì xóa thực sự
                voucher.IsDeleted = true;
                voucher.UpdatedAt = DateTime.UtcNow;

                await _voucherRepository.UpdateAsync(voucher);
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

        /// <summary>
        /// Lấy danh sách voucher có phân trang
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<Voucher>>> GetPaginatedVouchersAsync(int pageNumber, int pageSize)
        {
            try
            {
                // Lấy tổng số voucher
                var totalCount = await _voucherRepository.CountAsync();

                // Lấy danh sách voucher theo trang
                var vouchers = await _voucherRepository.GetPaginatedAsync(
                    filter: null,
                    orderBy: v => v.OrderByDescending(x => x.CreatedAt),
                    pageNumber: pageNumber,
                    pageSize: pageSize);

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    pageNumber,
                    pageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<Voucher>(
                    vouchers.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<Voucher>>.SuccessResponse(
                    paginatedResult,
                    "Lấy danh sách voucher thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher có phân trang: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<Voucher>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang với bộ lọc IsRoot
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<Voucher>>> GetPaginatedVouchersByRootAsync(int pageNumber, int pageSize, bool isRoot = true)
        {
            try
            {
                // Lấy tổng số voucher theo bộ lọc IsRoot
                var totalCount = await _voucherRepository.CountAsync(v => v.IsRoot == isRoot);

                // Lấy danh sách voucher theo trang và bộ lọc IsRoot
                var vouchers = await _voucherRepository.GetPaginatedAsync(
                    filter: v => v.IsRoot == isRoot,
                    orderBy: v => v.OrderByDescending(x => x.CreatedAt),
                    pageNumber: pageNumber,
                    pageSize: pageSize);

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    pageNumber,
                    pageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<Voucher>(
                    vouchers.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<Voucher>>.SuccessResponse(
                    paginatedResult,
                    $"Lấy danh sách voucher {(isRoot ? "gốc" : "con")} thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher có phân trang theo IsRoot: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<Voucher>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc nâng cao dành cho Admin hoặc Staff
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<Voucher>>> GetFilteredVouchersForAdminAsync(VoucherQueryRequest queryRequest)
        {
            try
            {
                // Gọi repository để lấy danh sách voucher đã lọc và phân trang
                var (vouchers, totalCount) = await _voucherRepository.GetFilteredVouchersAsync(queryRequest);

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    queryRequest.PageNumber,
                    queryRequest.PageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<Voucher>(
                    vouchers.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<Voucher>>.SuccessResponse(
                    paginatedResult,
                    "Lấy danh sách voucher đã lọc thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher có phân trang và lọc: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<Voucher>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách voucher gốc còn hiệu lực và còn lượt sử dụng
        /// </summary>
        /// <param name="filterRequest">Tham số lọc và phân trang</param>
        /// <returns>Danh sách voucher đã lọc và phân trang</returns>
        public async Task<ServiceResponse<PaginatedResult<Voucher>>> GetFilteredValidVouchersAsync(VoucherFilterRequest filterRequest)
        {
            try
            {
                // Gọi repository để lấy danh sách voucher đã lọc và phân trang
                var (vouchers, totalCount) = await _voucherRepository.GetFilteredValidVouchersAsync(filterRequest);

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    filterRequest.PageNumber,
                    filterRequest.PageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<Voucher>(
                    vouchers.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<Voucher>>.SuccessResponse(
                    paginatedResult,
                    "Lấy danh sách voucher hợp lệ thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher hợp lệ: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<Voucher>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher hợp lệ: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher của khách hàng đã lọc và phân trang</returns>
        public async Task<ServiceResponse<PaginatedResult<CustomerVoucherDTO>>> GetCustomerVouchersAsync(CustomerVoucherQueryRequest queryRequest, string token)
        {
            try
            {
                var customerId = _tokenService.GetUserIdFromToken(token);
                // Gọi repository để lấy danh sách voucher của khách hàng
                var (vouchers, totalCount) = await _voucherRepository.GetCustomerVouchersAsync(queryRequest, customerId);

                // Chuyển đổi từ Voucher sang CustomerVoucherDTO
                var customerVouchers = new List<CustomerVoucherDTO>();
                foreach (var voucher in vouchers)
                {
                    // Phân tích metadata để lấy thông tin về ngày nhận voucher
                    DateTime receivedDate = DateTime.UtcNow;
                    string description = "";

                    if (!string.IsNullOrEmpty(voucher.Metadata))
                    {
                        // Giả sử metadata có định dạng: "customerId:date:description"
                        var parts = voucher.Metadata.Split(':');
                        if (parts.Length >= 2)
                        {
                            // Phần tử thứ hai là ngày nhận
                            if (DateTime.TryParse(parts[1], out DateTime parsedDate))
                            {
                                receivedDate = parsedDate;
                            }

                            // Phần tử thứ ba là mô tả (nếu có)
                            if (parts.Length >= 3)
                            {
                                description = parts[2];
                            }
                        }
                    }

                    // Kiểm tra xem voucher đã hết hạn hay chưa
                    bool isExpired = (voucher.EndDate ?? DateTime.MaxValue) < DateTime.UtcNow;

                    // Kiểm tra xem voucher còn lượt sử dụng hay không
                    bool hasAvailableUsage = true;
                    if (voucher.UsageLimit.HasValue && voucher.UsedCount.HasValue)
                    {
                        hasAvailableUsage = voucher.UsedCount < voucher.UsageLimit;
                    }

                    customerVouchers.Add(new CustomerVoucherDTO
                    {
                        VoucherId = voucher.Voucherid,
                        Code = voucher.Code,
                        DiscountValue = voucher.DiscountValue ?? 0,
                        DiscountType = voucher.DiscountType ?? DiscountType.Percentage,
                        StartDate = voucher.StartDate ?? DateTime.UtcNow,
                        EndDate = voucher.EndDate ?? DateTime.UtcNow.AddDays(30),
                        MinOrder = voucher.MinOrder,
                        MaxDiscount = voucher.MaxDiscount,
                        CreatedAt = voucher.CreatedAt,
                        IsActive = voucher.IsActive ?? false,
                        Description = description,
                        ReceivedDate = receivedDate,
                        IsExpired = isExpired,
                        HasAvailableUsage = hasAvailableUsage
                    });
                }

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    queryRequest.PageNumber,
                    queryRequest.PageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<CustomerVoucherDTO>(
                    customerVouchers,
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<CustomerVoucherDTO>>.SuccessResponse(
                    paginatedResult,
                    "Lấy danh sách voucher của khách hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher của khách hàng: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<CustomerVoucherDTO>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher của khách hàng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PaginatedResult<CustomerVoucherDTO>>> GetCustomerVouchersForAdminAsync(CustomerVoucherQueryRequestForAdmin queryRequest)
        {
            try
            {
                var (vouchers, totalCount) = await _voucherRepository.GetCustomerVouchersForAdminAsync(queryRequest);

                // Chuyển đổi từ Voucher sang CustomerVoucherDTO
                var customerVouchers = new List<CustomerVoucherDTO>();
                foreach (var voucher in vouchers)
                {
                    // Phân tích metadata để lấy thông tin về ngày nhận voucher
                    DateTime receivedDate = DateTime.UtcNow;
                    string description = "";

                    if (!string.IsNullOrEmpty(voucher.Metadata))
                    {
                        // Giả sử metadata có định dạng: "customerId:date:description"
                        var parts = voucher.Metadata.Split(':');
                        if (parts.Length >= 2)
                        {
                            // Phần tử thứ hai là ngày nhận
                            if (DateTime.TryParse(parts[1], out DateTime parsedDate))
                            {
                                receivedDate = parsedDate;
                            }

                            // Phần tử thứ ba là mô tả (nếu có)
                            if (parts.Length >= 3)
                            {
                                description = parts[2];
                            }
                        }
                    }

                    // Kiểm tra xem voucher đã hết hạn hay chưa
                    bool isExpired = (voucher.EndDate ?? DateTime.MaxValue) < DateTime.UtcNow;

                    // Kiểm tra xem voucher còn lượt sử dụng hay không
                    bool hasAvailableUsage = true;
                    if (voucher.UsageLimit.HasValue && voucher.UsedCount.HasValue)
                    {
                        hasAvailableUsage = voucher.UsedCount < voucher.UsageLimit;
                    }

                    customerVouchers.Add(new CustomerVoucherDTO
                    {
                        VoucherId = voucher.Voucherid,
                        Code = voucher.Code,
                        DiscountValue = voucher.DiscountValue ?? 0,
                        DiscountType = voucher.DiscountType ?? DiscountType.Percentage,
                        StartDate = voucher.StartDate ?? DateTime.UtcNow,
                        EndDate = voucher.EndDate ?? DateTime.UtcNow.AddDays(30),
                        MinOrder = voucher.MinOrder,
                        MaxDiscount = voucher.MaxDiscount,
                        CreatedAt = voucher.CreatedAt,
                        IsActive = voucher.IsActive ?? false,
                        Description = description,
                        ReceivedDate = receivedDate,
                        IsExpired = isExpired,
                        HasAvailableUsage = hasAvailableUsage
                    });
                }

                // Tạo metadata cho phân trang
                var paginationMetadata = new PaginationMetadata(
                    queryRequest.PageNumber,
                    queryRequest.PageSize,
                    totalCount
                );

                // Tạo kết quả phân trang
                var paginatedResult = new PaginatedResult<CustomerVoucherDTO>(
                    customerVouchers,
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<CustomerVoucherDTO>>.SuccessResponse(
                    paginatedResult,
                    "Lấy danh sách voucher của khách hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách voucher của khách hàng: {Message}", ex.Message);
                return ServiceResponse<PaginatedResult<CustomerVoucherDTO>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách voucher của khách hàng: {ex.Message}");
            }
        }
    }
}