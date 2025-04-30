using System;
using System.Collections.Generic;

namespace WebTechnology.Repository.DTOs.Orders
{
    public class OrderRequestDTO
    {
        public string? ShippingAddress { get; set; }
        public decimal? ShippingFee { get; set; }
        public string? ShippingCode { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public string? StatusId { get; set; }
        public List<OrderDetailRequestDTO> OrderDetails { get; set; } = new List<OrderDetailRequestDTO>();
    }

    public class OrderDetailRequestDTO
    {
        public string ProductId { get; set; } = null!;
        public int Quantity { get; set; }
    }
} 