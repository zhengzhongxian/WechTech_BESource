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
    }
}
