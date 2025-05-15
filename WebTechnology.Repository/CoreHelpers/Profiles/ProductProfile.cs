using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.DTOs.Products;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, GetProductDTO>()
                .ForMember(dest => dest.Brand1, opt => opt.MapFrom(src => src.BrandNavigation != null ? src.BrandNavigation.Brand1 : null))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.BrandNavigation != null ? src.BrandNavigation.BrandName : null))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.UnitNavigation != null ? src.UnitNavigation.UnitName : null))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitNavigation != null ? src.UnitNavigation.Unit1 : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null))
                .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => src.SoldQuantity))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.PriceActive, opt => opt.MapFrom(src => src.ProductPrices != null
                    ? src.ProductPrices.Where(pp => pp.IsActive == true).Select(pp => pp.Price).FirstOrDefault()
                    : (decimal?)null))
                .ForMember(dest => dest.PriceDefault, opt => opt.MapFrom(src => src.ProductPrices != null
                    ? src.ProductPrices.Where(pp => pp.IsDefault == true).Select(pp => pp.Price).FirstOrDefault()
                    : (decimal?)null))
                .ForMember(dest => dest.imageDTOs, opt => opt.MapFrom(src => src.Images != null
                    ? src.Images.OrderBy(i => i.Order).Select(i => new ImageDTO
                    {
                        Imageid = i.Imageid,
                        ImageData = i.ImageData,
                        Order = i.Order
                    }).ToList()
                    : new List<ImageDTO>()))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions != null
                    ? src.Dimensions.Select(d => new DimensionDTO
                    {
                        DimensionId = d.Dimensionid,
                        WeightValue = d.WeightValue,
                        HeightValue = d.HeightValue,
                        WidthValue = d.WidthValue,
                        LengthValue = d.LengthValue
                    }).ToList()
                    : new List<DimensionDTO>()));
            CreateMap<Product, GetListProductTrends>().ReverseMap();
        }
    }
}
