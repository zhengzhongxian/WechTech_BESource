using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Statistics
{
    public class MonthlyRevenueDTO
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class YearlyRevenueDTO
    {
        public int Year { get; set; }
        public List<MonthlyRevenueDTO> MonthlyRevenues { get; set; } = new List<MonthlyRevenueDTO>();
        public decimal TotalRevenue { get; set; }
    }
}
