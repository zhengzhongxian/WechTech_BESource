using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Review
{
    public class ReviewDTO
    {
        public string ReviewId { get; set; }
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        //public string ProductName { get; set; }
        [Range(1, 5, ErrorMessage = "Chỉ được đánh giá từ 1 đến 5")]
        public int Rate { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 