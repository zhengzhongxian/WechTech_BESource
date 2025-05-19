using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.CoreHelpers;
using WebTechnology.Repository.DTOs.Statistics;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<ServiceResponse<YearlyRevenueDTO>> GetMonthlyRevenueForYearAsync(int year, string token);
        Task<ServiceResponse<MonthlySalesDTO>> GetProductSalesForMonthAsync(int month, int year, string token);

        /// <summary>
        /// Lấy số lượng khách hàng đang online
        /// </summary>
        Task<ServiceResponse<int>> GetOnlineCustomersCountAsync(string token);

        /// <summary>
        /// Lấy doanh thu trong ngày hôm nay
        /// </summary>
        Task<ServiceResponse<decimal>> GetTodayRevenueAsync(string token);

        /// <summary>
        /// Lấy số lượng sản phẩm bán được trong ngày hôm nay
        /// </summary>
        Task<ServiceResponse<int>> GetTodaySoldProductsCountAsync(string token);

        /// <summary>
        /// Lấy số lượng đơn hàng đang chờ xử lý (PENDING)
        /// </summary>
        Task<ServiceResponse<int>> GetPendingOrdersCountAsync(string token);
    }
}
