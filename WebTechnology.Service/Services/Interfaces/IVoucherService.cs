
using Microsoft.AspNetCore.JsonPatch;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<ServiceResponse<Voucher>> GetVoucherAsync(string idVoucher);
        Task<ServiceResponse<Voucher>> CreateVoucherAsync(CreateVoucherDTO createDto);
        Task<ServiceResponse<Voucher>> UpdateVoucherAsync(string idVoucher, JsonPatchDocument<Voucher> patchDoc);
        Task<ServiceResponse<bool>> DeleteVoucherAsync(string idVoucher);

        /// <summary>
        /// Lấy danh sách voucher có phân trang
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số lượng voucher trên mỗi trang</param>
        /// <returns>Danh sách voucher có phân trang</returns>
        Task<ServiceResponse<PaginatedResult<Voucher>>> GetPaginatedVouchersAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc nâng cao dành cho Admin hoặc Staff
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher đã lọc và phân trang</returns>
        Task<ServiceResponse<PaginatedResult<Voucher>>> GetFilteredVouchersForAdminAsync(VoucherQueryRequest queryRequest);

        /// <summary>
        /// Lấy danh sách voucher có phân trang với bộ lọc IsRoot
        /// </summary>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Số lượng voucher trên mỗi trang</param>
        /// <param name="isRoot">Lọc theo IsRoot (mặc định là true)</param>
        /// <returns>Danh sách voucher có phân trang</returns>
        Task<ServiceResponse<PaginatedResult<Voucher>>> GetPaginatedVouchersByRootAsync(int pageNumber, int pageSize, bool isRoot = true);

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher của khách hàng đã lọc và phân trang</returns>
        Task<ServiceResponse<PaginatedResult<CustomerVoucherDTO>>> GetCustomerVouchersAsync(CustomerVoucherQueryRequest queryRequest);
    }
}
