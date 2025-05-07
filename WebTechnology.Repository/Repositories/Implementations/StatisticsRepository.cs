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

        public async Task<MonthlySalesDTO> GetProductSalesForMonthAsync(int month, int year)
        {
            try
            {
                // In ra thông tin debug
                Console.WriteLine($"Đang tìm kiếm doanh số sản phẩm cho tháng {month}/{year}");

                // Lấy tất cả đơn hàng thành công trong tháng và năm chỉ định
                var ordersInMonth = await _context.Orders
                    .Where(o => o.IsSuccess == true &&
                           o.OrderDate.HasValue &&
                           o.OrderDate.Value.Month == month &&
                           o.OrderDate.Value.Year == year)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Include(o => o.ApplyVouchers)
                        .ThenInclude(av => av.Voucher)
                    .ToListAsync();

                Console.WriteLine($"Số đơn hàng trong tháng {month}/{year}: {ordersInMonth.Count}");

                // Tạo dictionary để tổng hợp doanh số theo sản phẩm
                var productSales = new Dictionary<string, ProductSalesDTO>();

                // Duyệt qua từng đơn hàng và chi tiết đơn hàng
                foreach (var order in ordersInMonth)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        if (detail.Product == null || detail.Quantity == null)
                            continue;

                        // Sử dụng giá trực tiếp từ OrderDetail
                        decimal price = detail.Price ?? 0;
                        decimal originalPrice = price;
                        decimal discountedPrice = price;

                        // Kiểm tra nếu đơn hàng có áp dụng voucher
                        if (order.ApplyVouchers != null && order.ApplyVouchers.Any())
                        {
                            // Tính tổng giá trị đơn hàng trước khi giảm giá
                            decimal orderTotal = order.OrderDetails.Sum(od => (od.Price ?? 0) * (od.Quantity ?? 0));

                            // Tính tổng giảm giá từ tất cả các voucher áp dụng
                            decimal totalDiscount = 0;

                            foreach (var applyVoucher in order.ApplyVouchers)
                            {
                                var voucher = applyVoucher.Voucher;
                                if (voucher == null) continue;

                                // Tính giảm giá
                                decimal discount = 0;
                                if (voucher.DiscountType == DiscountType.Percentage)
                                {
                                    // Giảm giá theo phần trăm
                                    discount = orderTotal * (voucher.DiscountValue ?? 0) / 100;

                                    // Áp dụng giới hạn giảm giá tối đa nếu có
                                    if (voucher.MaxDiscount.HasValue && discount > voucher.MaxDiscount.Value)
                                    {
                                        discount = voucher.MaxDiscount.Value;
                                    }
                                }
                                else if (voucher.DiscountType == DiscountType.FixedAmount)
                                {
                                    // Giảm giá cố định
                                    discount = voucher.DiscountValue ?? 0;
                                }

                                totalDiscount += discount;
                            }

                            // Đảm bảo tổng giảm giá không vượt quá tổng tiền đơn hàng
                            if (totalDiscount > orderTotal)
                            {
                                totalDiscount = orderTotal;
                            }

                            // Tính tỷ lệ giảm giá cho từng sản phẩm
                            if (orderTotal > 0)
                            {
                                decimal discountRatio = totalDiscount / orderTotal;
                                decimal itemDiscount = price * discountRatio;
                                discountedPrice = price - itemDiscount;
                            }
                        }

                        // Lấy hình ảnh đầu tiên của sản phẩm
                        var productImage = await _context.Images
                            .Where(i => i.Productid == detail.ProductId && i.Order == "1")
                            .FirstOrDefaultAsync();

                        string imageUrl = productImage?.ImageData ?? "";

                        // Tính tổng doanh số (sử dụng giá đã giảm)
                        decimal totalSale = discountedPrice * detail.Quantity.Value;

                        // Nếu sản phẩm đã có trong dictionary, cộng dồn số lượng và doanh số
                        if (productSales.ContainsKey(detail.ProductId))
                        {
                            productSales[detail.ProductId].Quantity += detail.Quantity.Value;
                            productSales[detail.ProductId].TotalSales += totalSale;
                        }
                        else
                        {
                            // Nếu chưa có, thêm mới vào dictionary
                            productSales.Add(detail.ProductId, new ProductSalesDTO
                            {
                                ProductId = detail.ProductId,
                                ProductName = detail.Product.ProductName,
                                ImageUrl = imageUrl,
                                Quantity = detail.Quantity.Value,
                                TotalSales = totalSale
                            });
                        }
                    }
                }

                // Chuyển dictionary thành danh sách và sắp xếp theo doanh số giảm dần
                var sortedProducts = productSales.Values
                    .OrderByDescending(p => p.TotalSales)
                    .ToList();

                // Tính tổng doanh số và số lượng
                decimal totalSales = sortedProducts.Sum(p => p.TotalSales);
                int totalQuantity = sortedProducts.Sum(p => p.Quantity);

                // Trả về kết quả
                return new MonthlySalesDTO
                {
                    Month = month,
                    Year = year,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Products = sortedProducts,
                    TotalSales = totalSales,
                    TotalQuantity = totalQuantity
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy doanh số sản phẩm theo tháng: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Trả về kết quả trống trong trường hợp lỗi
                return new MonthlySalesDTO
                {
                    Month = month,
                    Year = year,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Products = new List<ProductSalesDTO>(),
                    TotalSales = 0,
                    TotalQuantity = 0
                };
            }
        }

        public async Task<YearlyRevenueDTO> GetMonthlyRevenueForYearAsync(int year)
        {
            try
            {
                // In ra thông tin debug
                Console.WriteLine($"Đang tìm kiếm doanh thu cho năm: {year}");

                // Lấy tất cả đơn hàng thành công trong năm chỉ định
                var ordersInYear = await _context.Orders
                    .Where(o => o.IsSuccess == true &&
                           o.OrderDate.HasValue &&
                           o.OrderDate.Value.Year == year)
                    .ToListAsync();

                Console.WriteLine($"Tổng số đơn hàng thành công trong năm {year}: {ordersInYear.Count}");

                // In ra thông tin về các đơn hàng để debug
                foreach (var order in ordersInYear.Take(5)) // Chỉ lấy 5 đơn hàng đầu tiên để tránh quá nhiều log
                {
                    Console.WriteLine($"Order ID: {order.Orderid}, Date: {order.OrderDate}, TotalPrice: {order.TotalPrice}");
                }

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
