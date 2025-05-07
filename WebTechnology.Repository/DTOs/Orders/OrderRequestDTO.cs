using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebTechnology.Repository.DTOs.Orders
{
    public class OrderRequestDTO
    {
        public string? ShippingAddress { get; set; }
        public decimal? ShippingFee { get; set; }
        public string? ShippingCode { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        [JsonIgnore]
        public string? StatusId { get; set; } = "PENDING";
        public List<string> VoucherCodes { get; set; } = new List<string>();
        public List<OrderDetailRequestDTO> OrderDetails { get; set; } = new List<OrderDetailRequestDTO>();
    }

    public class OrderDetailRequestDTO
    {
        public string ProductId { get; set; } = null!;
        public int Quantity { get; set; }
    }
}