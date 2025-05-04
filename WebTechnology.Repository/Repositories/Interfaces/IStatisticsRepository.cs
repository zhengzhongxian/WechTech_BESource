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
    }
}
