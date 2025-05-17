using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly WebTech _webTech;
        public VoucherRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }

        public async Task<IEnumerable<Voucher>> FindAsync(Expression<Func<Voucher, bool>> predicate)
        {
            return await _webTech.Vouchers
                .Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Đếm tổng số voucher
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<Voucher, bool>> predicate = null)
        {
            IQueryable<Voucher> query = _webTech.Vouchers;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc nâng cao
        /// </summary>
        public async Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetFilteredVouchersAsync(VoucherQueryRequest queryRequest)
        {
            // Chỉ lấy voucher không bị xóa
            IQueryable<Voucher> query = _webTech.Vouchers.Where(v => v.IsDeleted != true);

            // Áp dụng các bộ lọc
            if (!string.IsNullOrWhiteSpace(queryRequest.SearchTerm))
            {
                query = query.Where(v => v.Code != null && v.Code.Contains(queryRequest.SearchTerm));
            }

            if (queryRequest.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == queryRequest.IsActive.Value);
            }

            if (queryRequest.DiscountType.HasValue)
            {
                query = query.Where(v => v.DiscountType == queryRequest.DiscountType.Value);
            }

            if (queryRequest.IsRoot.HasValue)
            {
                query = query.Where(v => v.IsRoot == queryRequest.IsRoot.Value);
            }

            // Lọc theo ngày bắt đầu
            if (queryRequest.StartDateFrom.HasValue)
            {
                query = query.Where(v => v.StartDate >= queryRequest.StartDateFrom.Value);
            }

            if (queryRequest.StartDateTo.HasValue)
            {
                query = query.Where(v => v.StartDate <= queryRequest.StartDateTo.Value);
            }

            // Lọc theo ngày kết thúc
            if (queryRequest.EndDateFrom.HasValue)
            {
                query = query.Where(v => v.EndDate >= queryRequest.EndDateFrom.Value);
            }

            if (queryRequest.EndDateTo.HasValue)
            {
                query = query.Where(v => v.EndDate <= queryRequest.EndDateTo.Value);
            }

            // Đếm tổng số bản ghi sau khi lọc
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, queryRequest.SortBy, queryRequest.SortAscending);

            // Áp dụng phân trang
            var vouchers = await query
                .Skip((queryRequest.PageNumber - 1) * queryRequest.PageSize)
                .Take(queryRequest.PageSize)
                .ToListAsync();

            return (vouchers, totalCount);
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang
        /// </summary>
        public async Task<IEnumerable<Voucher>> GetPaginatedAsync(
            Expression<Func<Voucher, bool>> filter = null,
            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // Chỉ lấy voucher không bị xóa
            IQueryable<Voucher> query = _webTech.Vouchers.Where(v => v.IsDeleted != true);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                // Mặc định sắp xếp theo ngày tạo giảm dần
                query = query.OrderByDescending(v => v.CreatedAt);
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Áp dụng sắp xếp cho truy vấn voucher
        /// </summary>
        private IQueryable<Voucher> ApplySorting(IQueryable<Voucher> query, string sortBy, bool sortAscending)
        {
            switch (sortBy?.ToLower())
            {
                case "code":
                    return sortAscending
                        ? query.OrderBy(v => v.Code)
                        : query.OrderByDescending(v => v.Code);
                case "discountvalue":
                    return sortAscending
                        ? query.OrderBy(v => v.DiscountValue)
                        : query.OrderByDescending(v => v.DiscountValue);
                case "startdate":
                    return sortAscending
                        ? query.OrderBy(v => v.StartDate)
                        : query.OrderByDescending(v => v.StartDate);
                case "enddate":
                    return sortAscending
                        ? query.OrderBy(v => v.EndDate)
                        : query.OrderByDescending(v => v.EndDate);
                case "usagelimit":
                    return sortAscending
                        ? query.OrderBy(v => v.UsageLimit)
                        : query.OrderByDescending(v => v.UsageLimit);
                case "usedcount":
                    return sortAscending
                        ? query.OrderBy(v => v.UsedCount)
                        : query.OrderByDescending(v => v.UsedCount);
                case "point":
                    return sortAscending
                        ? query.OrderBy(v => v.Point)
                        : query.OrderByDescending(v => v.Point);
                case "createdat":
                default:
                    return sortAscending
                        ? query.OrderBy(v => v.CreatedAt)
                        : query.OrderByDescending(v => v.CreatedAt);
            }
        }

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher của khách hàng đã lọc và phân trang</returns>
        public async Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetCustomerVouchersAsync(CustomerVoucherQueryRequest queryRequest, string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return (new List<Voucher>(), 0);
            }

            // Lấy ngày hiện tại
            var currentDate = DateTime.UtcNow;

            // Lấy tất cả voucher có metadata chứa customerId, không bị xóa, còn hạn và còn lượt sử dụng
            IQueryable<Voucher> query = _webTech.Vouchers
                // Không bị xóa
                .Where(v => v.IsDeleted != true)
                // Có metadata chứa customerId
                .Where(v => v.Metadata != null && v.Metadata.Contains(customerId))
                // Chưa hết hạn
                .Where(v => v.EndDate > currentDate)
                // Còn lượt sử dụng
                .Where(v => v.UsageLimit == null || v.UsedCount < v.UsageLimit);

            // Áp dụng các bộ lọc
            if (!string.IsNullOrWhiteSpace(queryRequest.SearchTerm))
            {
                query = query.Where(v => v.Code != null && v.Code.Contains(queryRequest.SearchTerm));
            }

            if (queryRequest.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == queryRequest.IsActive.Value);
            }

            if (queryRequest.DiscountType.HasValue)
            {
                var discountTypeValue = (DiscountType)queryRequest.DiscountType.Value;
                query = query.Where(v => v.DiscountType == discountTypeValue);
            }

            // Lọc theo ngày bắt đầu
            if (queryRequest.StartDateFrom.HasValue)
            {
                query = query.Where(v => v.StartDate >= queryRequest.StartDateFrom.Value);
            }

            if (queryRequest.StartDateTo.HasValue)
            {
                query = query.Where(v => v.StartDate <= queryRequest.StartDateTo.Value);
            }

            // Lọc theo ngày kết thúc
            if (queryRequest.EndDateFrom.HasValue)
            {
                query = query.Where(v => v.EndDate >= queryRequest.EndDateFrom.Value);
            }

            if (queryRequest.EndDateTo.HasValue)
            {
                query = query.Where(v => v.EndDate <= queryRequest.EndDateTo.Value);
            }

            // Đếm tổng số bản ghi sau khi lọc
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, queryRequest.SortBy, queryRequest.SortAscending);

            // Áp dụng phân trang
            var vouchers = await query
                .Skip((queryRequest.PageNumber - 1) * queryRequest.PageSize)
                .Take(queryRequest.PageSize)
                .ToListAsync();

            return (vouchers, totalCount);
        }
        /// <summary>
        /// Lấy danh sách voucher gốc còn hiệu lực và còn lượt sử dụng
        /// </summary>
        /// <param name="filterRequest">Tham số lọc và phân trang</param>
        /// <returns>Danh sách voucher đã lọc và phân trang</returns>
        public async Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetFilteredValidVouchersAsync(VoucherFilterRequest filterRequest)
        {
            // Lấy ngày hiện tại
            var currentDate = DateTime.UtcNow;

            // Tạo truy vấn cơ bản
            IQueryable<Voucher> query = _webTech.Vouchers
                // Chỉ lấy voucher không bị xóa
                .Where(v => v.IsDeleted != true)
                // Chỉ lấy voucher gốc
                .Where(v => v.IsRoot == true)
                // Chỉ lấy voucher còn hiệu lực
                .Where(v => v.IsActive == true)
                // Chỉ lấy voucher chưa hết hạn
                .Where(v => v.EndDate > currentDate)
                // Chỉ lấy voucher còn lượt sử dụng
                .Where(v => v.UsageLimit == null || v.UsedCount < v.UsageLimit);

            // Áp dụng tìm kiếm theo mã voucher nếu có
            if (!string.IsNullOrWhiteSpace(filterRequest.SearchTerm))
            {
                query = query.Where(v => v.Code != null && v.Code.Contains(filterRequest.SearchTerm));
            }

            // Đếm tổng số bản ghi sau khi lọc
            var totalCount = await query.CountAsync();

            // Mặc định sắp xếp theo ngày tạo giảm dần
            query = query.OrderByDescending(v => v.CreatedAt);

            // Áp dụng phân trang
            var vouchers = await query
                .Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                .Take(filterRequest.PageSize)
                .ToListAsync();

            return (vouchers, totalCount);
        }

        public async Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetCustomerVouchersForAdminAsync(CustomerVoucherQueryRequestForAdmin queryRequest)
        {
            if (string.IsNullOrEmpty(queryRequest.CustomerId))
            {
                return (new List<Voucher>(), 0);
            }

            // Lấy tất cả voucher có metadata chứa customerId và không bị xóa
            IQueryable<Voucher> query = _webTech.Vouchers
                .Where(v => v.IsDeleted != true)
                .Where(v => v.Metadata != null && v.Metadata.Contains(queryRequest.CustomerId));

            // Áp dụng các bộ lọc
            if (!string.IsNullOrWhiteSpace(queryRequest.SearchTerm))
            {
                query = query.Where(v => v.Code != null && v.Code.Contains(queryRequest.SearchTerm));
            }

            if (queryRequest.IsActive.HasValue)
            {
                query = query.Where(v => v.IsActive == queryRequest.IsActive.Value);
            }

            if (queryRequest.DiscountType.HasValue)
            {
                var discountTypeValue = (DiscountType)queryRequest.DiscountType.Value;
                query = query.Where(v => v.DiscountType == discountTypeValue);
            }

            // Lọc theo ngày bắt đầu
            if (queryRequest.StartDateFrom.HasValue)
            {
                query = query.Where(v => v.StartDate >= queryRequest.StartDateFrom.Value);
            }

            if (queryRequest.StartDateTo.HasValue)
            {
                query = query.Where(v => v.StartDate <= queryRequest.StartDateTo.Value);
            }

            // Lọc theo ngày kết thúc
            if (queryRequest.EndDateFrom.HasValue)
            {
                query = query.Where(v => v.EndDate >= queryRequest.EndDateFrom.Value);
            }

            if (queryRequest.EndDateTo.HasValue)
            {
                query = query.Where(v => v.EndDate <= queryRequest.EndDateTo.Value);
            }

            // Đếm tổng số bản ghi sau khi lọc
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, queryRequest.SortBy, queryRequest.SortAscending);

            // Áp dụng phân trang
            var vouchers = await query
                .Skip((queryRequest.PageNumber - 1) * queryRequest.PageSize)
                .Take(queryRequest.PageSize)
                .ToListAsync();

            return (vouchers, totalCount);
        }
    }
}
