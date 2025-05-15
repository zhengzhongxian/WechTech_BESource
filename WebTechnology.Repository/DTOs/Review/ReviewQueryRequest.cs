using System;

namespace WebTechnology.Repository.DTOs.Review
{
    public class ReviewQueryRequest
    {
        /// <summary>
        /// Số trang (bắt đầu từ 1)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng mục trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// ID sản phẩm cần lấy đánh giá
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Sắp xếp theo (CreatedAt, Rate)
        /// </summary>
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sắp xếp tăng dần (true) hoặc giảm dần (false)
        /// </summary>
        public bool SortAscending { get; set; } = false;

        /// <summary>
        /// Lọc theo số sao đánh giá (1-5)
        /// </summary>
        public int? RateFilter { get; set; }
    }
}
