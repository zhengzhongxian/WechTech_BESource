using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.UserStatuses;

namespace WebTechnology.Repository.Mappings
{
    public class UserStatusMappingProfile : Profile
    {
        public UserStatusMappingProfile()
        {
            // Map from CreateUserStatusDTO to UserStatus
            CreateMap<CreateUserStatusDTO, UserStatus>()
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());
        }
    }
}