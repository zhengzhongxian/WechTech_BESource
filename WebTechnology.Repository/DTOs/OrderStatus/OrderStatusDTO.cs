using System;

namespace WebTechnology.Repository.DTOs.OrderStatus
{
    public class OrderStatusDTO
    {
        public string StatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
