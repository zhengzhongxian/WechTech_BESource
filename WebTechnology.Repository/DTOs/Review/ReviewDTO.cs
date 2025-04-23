namespace WebTechnology.Repository.DTOs.Review
{
    public class ReviewDTO
    {
        public string ReviewId { get; set; }
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        //public string ProductName { get; set; }
        public int Rate { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 