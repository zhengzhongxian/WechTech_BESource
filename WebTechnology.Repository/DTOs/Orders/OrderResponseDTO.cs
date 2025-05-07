using System;
using System.Collections.Generic;

namespace WebTechnology.Repository.DTOs.Orders
{
    public class OrderResponseDTO
    {
        public string OrderId { get; set; } = null!;
        public string? OrderNumber { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal? ShippingFee { get; set; }
        public string? ShippingCode { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? StatusId { get; set; }
        public bool? IsSuccess { get; set; }
        public List<OrderDetailResponseDTO> OrderDetails { get; set; } = new List<OrderDetailResponseDTO>();
    }

    public class OrderDetailResponseDTO
    {
        public string OrderDetailId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
        public string? Img { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? SubTotal { get; set; }
    }
} 