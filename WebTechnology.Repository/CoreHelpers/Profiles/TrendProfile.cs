using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Trends;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class TrendProfile : Profile
    {
        public TrendProfile()
        {
            CreateMap<CreateTrendDTO, Trend>().ReverseMap();
        }
    }
}
