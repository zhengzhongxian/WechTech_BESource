using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Vouchers;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IVoucherRepository : IGenericRepository<Voucher>
    {
        Task<IEnumerable<Voucher>> FindAsync(Expression<Func<Voucher, bool>> predicate);

        /// <summary>
        /// Đếm tổng số voucher
        /// </summary>
        Task<int> CountAsync(Expression<Func<Voucher, bool>> predicate = null);

        /// <summary>
        /// Lấy danh sách voucher có phân trang
        /// </summary>
        Task<IEnumerable<Voucher>> GetPaginatedAsync(
            Expression<Func<Voucher, bool>> filter = null,
            Func<IQueryable<Voucher>, IOrderedQueryable<Voucher>> orderBy = null,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc nâng cao
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher đã lọc và phân trang</returns>
        Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetFilteredVouchersAsync(VoucherQueryRequest queryRequest);

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <param name="queryRequest">Tham số truy vấn và lọc</param>
        /// <returns>Danh sách voucher của khách hàng đã lọc và phân trang</returns>
        Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetCustomerVouchersAsync(CustomerVoucherQueryRequest queryRequest, string customerId);
        Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetCustomerVouchersForAdminAsync(CustomerVoucherQueryRequestForAdmin queryRequest);

        /// <summary>
        /// Lấy danh sách voucher gốc còn hiệu lực và còn lượt sử dụng
        /// </summary>
        /// <param name="filterRequest">Tham số lọc và phân trang</param>
        /// <returns>Danh sách voucher đã lọc và phân trang</returns>
        Task<(IEnumerable<Voucher> Vouchers, int TotalCount)> GetFilteredValidVouchersAsync(VoucherFilterRequest filterRequest);
    }
}
