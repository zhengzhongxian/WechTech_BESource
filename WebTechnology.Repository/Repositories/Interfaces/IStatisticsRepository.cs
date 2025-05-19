using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Statistics;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IStatisticsRepository
    {
        Task<YearlyRevenueDTO> GetMonthlyRevenueForYearAsync(int year);
        Task<MonthlySalesDTO> GetProductSalesForMonthAsync(int month, int year);

        /// <summary>
        /// Lấy số lượng khách hàng đang online
        /// </summary>
        Task<int> GetOnlineCustomersCountAsync();

        /// <summary>
        /// Lấy doanh thu trong ngày hôm nay
        /// </summary>
        Task<decimal> GetTodayRevenueAsync();

        /// <summary>
        /// Lấy số lượng sản phẩm bán được trong ngày hôm nay
        /// </summary>
        Task<int> GetTodaySoldProductsCountAsync();

        /// <summary>
        /// Lấy số lượng đơn hàng đang chờ xử lý (PENDING)
        /// </summary>
        Task<int> GetPendingOrdersCountAsync();
    }
}
