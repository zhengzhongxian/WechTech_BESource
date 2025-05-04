using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Orders;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Orderid))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderDetailResponseDTO>()
                .ForMember(dest => dest.OrderDetailId, opt => opt.MapFrom(src => src.OrderDetailId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => 
                    src.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true) != null 
                        ? src.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true).Price 
                        : 0))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => 
                    src.Quantity * (src.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true) != null 
                        ? src.Product.ProductPrices.FirstOrDefault(pp => pp.IsActive == true).Price 
                        : 0)));
        }
    }
} 