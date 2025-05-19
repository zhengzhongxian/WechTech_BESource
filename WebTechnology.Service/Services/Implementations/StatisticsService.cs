using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.CoreHelpers;
using WebTechnology.Repository.DTOs.Statistics;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<ServiceResponse<YearlyRevenueDTO>> GetMonthlyRevenueForYearAsync(int year, string token)
        {
            try
            {
                // Kiểm tra năm hợp lệ
                if (year < 2000 || year > DateTime.Now.Year)
                {
                    return ServiceResponse<YearlyRevenueDTO>.FailResponse(
                        "Năm không hợp lệ");
                }

                // Lấy dữ liệu doanh thu theo tháng
                var yearlyRevenue = await _statisticsRepository.GetMonthlyRevenueForYearAsync(year);

                return ServiceResponse<YearlyRevenueDTO>.SuccessResponse(
                    yearlyRevenue,
                    $"Lấy doanh thu theo tháng trong năm {year} thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<YearlyRevenueDTO>.ErrorResponse(
                    $"Lỗi khi lấy doanh thu theo tháng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<MonthlySalesDTO>> GetProductSalesForMonthAsync(int month, int year, string token)
        {
            try
            {
                // Kiểm tra tháng hợp lệ
                if (month < 1 || month > 12)
                {
                    return ServiceResponse<MonthlySalesDTO>.FailResponse(
                        "Tháng không hợp lệ. Tháng phải từ 1 đến 12");
                }

                // Kiểm tra năm hợp lệ
                if (year < 2000 || year > DateTime.Now.Year)
                {
                    return ServiceResponse<MonthlySalesDTO>.FailResponse(
                        "Năm không hợp lệ");
                }

                // Kiểm tra tháng và năm không vượt quá thời gian hiện tại
                if (year == DateTime.Now.Year && month > DateTime.Now.Month)
                {
                    return ServiceResponse<MonthlySalesDTO>.FailResponse(
                        "Tháng và năm không được vượt quá thời gian hiện tại");
                }

                // Lấy dữ liệu doanh số sản phẩm theo tháng
                var monthlySales = await _statisticsRepository.GetProductSalesForMonthAsync(month, year);

                return ServiceResponse<MonthlySalesDTO>.SuccessResponse(
                    monthlySales,
                    $"Lấy doanh số sản phẩm trong tháng {month}/{year} thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<MonthlySalesDTO>.ErrorResponse(
                    $"Lỗi khi lấy doanh số sản phẩm theo tháng: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy doanh thu của một sản phẩm theo từng tháng trong năm
        /// </summary>
        public async Task<ServiceResponse<ProductYearlyRevenueDTO>> GetProductMonthlyRevenueForYearAsync(string productId, int year, string token)
        {
            try
            {
                // Kiểm tra productId
                if (string.IsNullOrEmpty(productId))
                {
                    return ServiceResponse<ProductYearlyRevenueDTO>.FailResponse(
                        "ID sản phẩm không được để trống");
                }

                // Kiểm tra năm hợp lệ
                if (year < 2000 || year > DateTime.Now.Year)
                {
                    return ServiceResponse<ProductYearlyRevenueDTO>.FailResponse(
                        "Năm không hợp lệ");
                }

                // Lấy dữ liệu doanh thu sản phẩm theo tháng
                var productYearlyRevenue = await _statisticsRepository.GetProductMonthlyRevenueForYearAsync(productId, year);

                return ServiceResponse<ProductYearlyRevenueDTO>.SuccessResponse(
                    productYearlyRevenue,
                    $"Lấy doanh thu sản phẩm theo tháng trong năm {year} thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<ProductYearlyRevenueDTO>.ErrorResponse(
                    $"Lỗi khi lấy doanh thu sản phẩm theo tháng: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy số lượng khách hàng đang online
        /// </summary>
        public async Task<ServiceResponse<int>> GetOnlineCustomersCountAsync(string token)
        {
            try
            {
                // Lấy số lượng khách hàng đang online
                var onlineCustomersCount = await _statisticsRepository.GetOnlineCustomersCountAsync();

                return ServiceResponse<int>.SuccessResponse(
                    onlineCustomersCount,
                    "Lấy số lượng khách hàng đang online thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.ErrorResponse(
                    $"Lỗi khi lấy số lượng khách hàng đang online: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy doanh thu trong ngày hôm nay
        /// </summary>
        public async Task<ServiceResponse<decimal>> GetTodayRevenueAsync(string token)
        {
            try
            {
                // Lấy doanh thu trong ngày hôm nay
                var todayRevenue = await _statisticsRepository.GetTodayRevenueAsync();

                return ServiceResponse<decimal>.SuccessResponse(
                    todayRevenue,
                    "Lấy doanh thu trong ngày hôm nay thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<decimal>.ErrorResponse(
                    $"Lỗi khi lấy doanh thu trong ngày hôm nay: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy số lượng sản phẩm bán được trong ngày hôm nay
        /// </summary>
        public async Task<ServiceResponse<int>> GetTodaySoldProductsCountAsync(string token)
        {
            try
            {
                // Lấy số lượng sản phẩm bán được trong ngày hôm nay
                var todaySoldProductsCount = await _statisticsRepository.GetTodaySoldProductsCountAsync();

                return ServiceResponse<int>.SuccessResponse(
                    todaySoldProductsCount,
                    "Lấy số lượng sản phẩm bán được trong ngày hôm nay thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.ErrorResponse(
                    $"Lỗi khi lấy số lượng sản phẩm bán được trong ngày hôm nay: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy số lượng đơn hàng đang chờ xử lý (PENDING)
        /// </summary>
        public async Task<ServiceResponse<int>> GetPendingOrdersCountAsync(string token)
        {
            try
            {
                // Lấy số lượng đơn hàng đang chờ xử lý (PENDING)
                var pendingOrdersCount = await _statisticsRepository.GetPendingOrdersCountAsync();

                return ServiceResponse<int>.SuccessResponse(
                    pendingOrdersCount,
                    "Lấy số lượng đơn hàng đang chờ xử lý thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<int>.ErrorResponse(
                    $"Lỗi khi lấy số lượng đơn hàng đang chờ xử lý: {ex.Message}");
            }
        }
    }
}
