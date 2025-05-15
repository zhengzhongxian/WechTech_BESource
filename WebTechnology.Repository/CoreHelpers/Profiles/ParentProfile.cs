using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Parents;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class ParentProfile : Profile
    {
        public ParentProfile()
        {
            CreateMap<Parent, ParentDTO>().ReverseMap();
            CreateMap<CreateParentDTO, Parent>();
        }
    }
}
