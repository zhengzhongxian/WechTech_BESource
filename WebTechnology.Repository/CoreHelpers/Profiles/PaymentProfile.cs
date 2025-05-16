using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Payments;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Paymentid))
                .ForMember(dest => dest.PaymentName, opt => opt.MapFrom(src => src.PaymentName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
        }
    }
}
