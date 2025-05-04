using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Statistics;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly WebTech _context;

        public StatisticsRepository(WebTech context)
        {
            _context = context;
        }

        public async Task<YearlyRevenueDTO> GetMonthlyRevenueForYearAsync(int year)
        {
            try
            {
                // In ra thông tin debug
                Console.WriteLine($"Đang tìm kiếm doanh thu cho năm: {year}");

                // Lấy tất cả đơn hàng
                var allOrders = await _context.Orders.ToListAsync();
                Console.WriteLine($"Tổng số đơn hàng tìm thấy: {allOrders.Count}");

                // In ra thông tin về các đơn hàng để debug
                foreach (var order in allOrders.Take(5)) // Chỉ lấy 5 đơn hàng đầu tiên để tránh quá nhiều log
                {
                    Console.WriteLine($"Order ID: {order.Orderid}, Date: {order.OrderDate}, TotalPrice: {order.TotalPrice}, IsSuccess: {order.IsSuccess}");

                    // Kiểm tra định dạng ngày tháng
                    if (order.OrderDate.HasValue)
                    {
                        Console.WriteLine($"  - OrderDate.Year: {order.OrderDate.Value.Year}");
                        Console.WriteLine($"  - OrderDate.Month: {order.OrderDate.Value.Month}");
                        Console.WriteLine($"  - OrderDate.Day: {order.OrderDate.Value.Day}");
                    }
                    else
                    {
                        Console.WriteLine($"  - OrderDate is null");

                        // Kiểm tra các thuộc tính khác có thể chứa thông tin ngày tháng
                        var properties = order.GetType().GetProperties();
                        foreach (var prop in properties)
                        {
                            if (prop.Name.Contains("Date") || prop.Name.Contains("Time"))
                            {
                                var value = prop.GetValue(order);
                                Console.WriteLine($"  - {prop.Name}: {value}");
                            }
                        }
                    }
                }

                // Lấy tất cả đơn hàng thành công
                var successOrders = allOrders.Where(o => o.IsSuccess == true).ToList();
                Console.WriteLine($"Tổng số đơn hàng thành công: {successOrders.Count}");

                // Lọc đơn hàng theo năm - sử dụng LINQ để code ngắn gọn hơn
                var ordersInYear = successOrders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                    .ToList();

                // In ra thông tin về các đơn hàng trong năm
                foreach (var order in ordersInYear.Take(5)) // Chỉ lấy 5 đơn hàng đầu tiên để tránh quá nhiều log
                {
                    Console.WriteLine($"Đơn hàng trong năm {year}: ID={order.Orderid}, Date={order.OrderDate}, TotalPrice={order.TotalPrice}");
                }

                Console.WriteLine($"Số đơn hàng trong năm {year}: {ordersInYear.Count}");

                // Tạo danh sách các tháng trong năm
                var monthlyRevenues = new List<MonthlyRevenueDTO>();
                for (int month = 1; month <= 12; month++)
                {
                    // Lọc đơn hàng theo tháng - sử dụng LINQ để code ngắn gọn hơn
                    var ordersInMonth = ordersInYear
                        .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Month == month)
                        .ToList();

                    Console.WriteLine($"Tháng {month}: {ordersInMonth.Count} đơn hàng");

                    // Tính tổng doanh thu trong tháng
                    decimal revenue = ordersInMonth.Sum(o => o.TotalPrice ?? 0);

                    Console.WriteLine($"Doanh thu tháng {month}: {revenue}");

                    // Thêm vào danh sách kết quả
                    monthlyRevenues.Add(new MonthlyRevenueDTO
                    {
                        Month = month,
                        MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        Revenue = revenue
                    });
                }

                // Tính tổng doanh thu trong năm
                decimal totalRevenue = monthlyRevenues.Sum(m => m.Revenue);

                Console.WriteLine($"Tổng doanh thu năm {year}: {totalRevenue}");

                // Trả về kết quả
                return new YearlyRevenueDTO
                {
                    Year = year,
                    MonthlyRevenues = monthlyRevenues,
                    TotalRevenue = totalRevenue
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy doanh thu theo tháng: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Trả về kết quả trống trong trường hợp lỗi
                return new YearlyRevenueDTO
                {
                    Year = year,
                    MonthlyRevenues = new List<MonthlyRevenueDTO>(),
                    TotalRevenue = 0
                };
            }
        }
    }
}
