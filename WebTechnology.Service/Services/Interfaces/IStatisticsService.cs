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
    }
}
