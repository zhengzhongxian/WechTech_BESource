using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.OrderStatus;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class OrderStatusProfile : Profile
    {
        public OrderStatusProfile()
        {
            CreateMap<OrderStatus, OrderStatusDTO>();
        }
    }
}
