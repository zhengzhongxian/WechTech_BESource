using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        /// <summary>
        /// Lấy voucher theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucher(string id)
        {
            var response = await _voucherService.GetVoucherAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách voucher gốc còn hiệu lực và còn lượt sử dụng
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các voucher gốc (IsRoot = true) còn hiệu lực (IsActive = true),
        /// chưa hết hạn (EndDate > ngày hiện tại) và còn lượt sử dụng (UsedCount < UsageLimit).
        ///
        /// Các tham số:
        /// - pageNumber: Số trang (bắt đầu từ 1)
        /// - pageSize: Số lượng voucher mỗi trang
        /// - searchTerm: Từ khóa tìm kiếm theo mã voucher
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetVouchers([FromQuery] VoucherFilterRequest filterRequest)
        {
            var response = await _voucherService.GetFilteredValidVouchersAsync(filterRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang theo IsRoot
        /// </summary>
        [HttpGet("by-root")]
        public async Task<IActionResult> GetVouchersByRoot(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isRoot = true)
        {
            var response = await _voucherService.GetPaginatedVouchersByRootAsync(pageNumber, pageSize, isRoot);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc nâng cao dành cho Admin hoặc Staff
        /// </summary>
        /// <remarks>
        /// API này cho phép lọc và phân trang danh sách voucher với nhiều tiêu chí khác nhau.
        ///
        /// Các tham số lọc:
        /// - SearchTerm: Tìm kiếm theo mã voucher
        /// - IsActive: Lọc theo trạng thái hoạt động (true/false)
        /// - DiscountType: Lọc theo loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
        /// - IsRoot: Lọc theo voucher gốc (true) hoặc voucher con (false)
        /// - StartDateFrom/StartDateTo: Lọc theo khoảng thời gian bắt đầu
        /// - EndDateFrom/EndDateTo: Lọc theo khoảng thời gian kết thúc
        ///
        /// Các tham số sắp xếp:
        /// - SortBy: Sắp xếp theo trường (CreatedAt, Code, DiscountValue, StartDate, EndDate, UsageLimit, UsedCount, Point)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </remarks>
        [HttpGet("admin-staff")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetFilteredVouchersForAdmin([FromQuery] VoucherQueryRequest queryRequest)
        {
            var response = await _voucherService.GetFilteredVouchersForAdminAsync(queryRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDTO createDto)
        {
            var response = await _voucherService.CreateVoucherAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> UpdateVoucher(string id, [FromBody] JsonPatchDocument<Voucher> patchDoc)
        {
            var response = await _voucherService.UpdateVoucherAsync(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            var response = await _voucherService.DeleteVoucherAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <remarks>
        /// API này cho phép lấy danh sách voucher của một khách hàng cụ thể từ metadata với phân trang và lọc.
        ///
        /// Các tham số bắt buộc:
        /// - CustomerId: ID của khách hàng
        ///
        /// Các tham số lọc:
        /// - SearchTerm: Tìm kiếm theo mã voucher
        /// - IsActive: Lọc theo trạng thái hoạt động (true/false)
        /// - DiscountType: Lọc theo loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
        /// - StartDateFrom/StartDateTo: Lọc theo khoảng thời gian bắt đầu
        /// - EndDateFrom/EndDateTo: Lọc theo khoảng thời gian kết thúc
        ///
        /// Các tham số sắp xếp:
        /// - SortBy: Sắp xếp theo trường (CreatedAt, Code, DiscountValue, StartDate, EndDate)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </remarks>

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng lấy danh sách voucher của mình từ metadata với phân trang và lọc.
        /// CustomerId được lấy tự động từ token JWT, không cần truyền vào.
        ///
        /// Các tham số lọc:
        /// - SearchTerm: Tìm kiếm theo mã voucher
        /// - IsActive: Lọc theo trạng thái hoạt động (true/false)
        /// - DiscountType: Lọc theo loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
        /// - StartDateFrom/StartDateTo: Lọc theo khoảng thời gian bắt đầu
        /// - EndDateFrom/EndDateTo: Lọc theo khoảng thời gian kết thúc
        ///
        /// Các tham số sắp xếp:
        /// - SortBy: Sắp xếp theo trường (CreatedAt, Code, DiscountValue, StartDate, EndDate)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </remarks>
        [HttpGet("customer-vouchers")]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> GetCustomerVouchers([FromQuery] CustomerVoucherQueryRequest queryRequest)
        {
            var token = Request.Headers["Authorization"].ToString();
            var response = await _voucherService.GetCustomerVouchersAsync(queryRequest, token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata (dành cho Admin/Staff)
        /// </summary>
        /// <remarks>
        /// API này cho phép Admin hoặc Staff lấy danh sách voucher của một khách hàng cụ thể từ metadata với phân trang và lọc.
        ///
        /// Các tham số bắt buộc:
        /// - CustomerId: ID của khách hàng
        ///
        /// Các tham số lọc:
        /// - SearchTerm: Tìm kiếm theo mã voucher
        /// - IsActive: Lọc theo trạng thái hoạt động (true/false)
        /// - DiscountType: Lọc theo loại giảm giá (0: Phần trăm, 1: Giá trị cố định)
        /// - StartDateFrom/StartDateTo: Lọc theo khoảng thời gian bắt đầu
        /// - EndDateFrom/EndDateTo: Lọc theo khoảng thời gian kết thúc
        ///
        /// Các tham số sắp xếp:
        /// - SortBy: Sắp xếp theo trường (CreatedAt, Code, DiscountValue, StartDate, EndDate)
        /// - SortAscending: Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </remarks>
        [HttpGet("admin-staff/customer-vouchers")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> GetCustomerVouchersForAdmin([FromQuery] CustomerVoucherQueryRequestForAdmin queryRequest)
        {
            var response = await _voucherService.GetCustomerVouchersForAdminAsync(queryRequest);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}