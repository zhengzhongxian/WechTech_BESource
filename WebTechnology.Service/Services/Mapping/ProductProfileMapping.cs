using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Products;

namespace WebTechnology.Repository.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Map from CreateProductDTO to Product
            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.Productid, opt => opt.Ignore())
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Stockquantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.Bar, opt => opt.MapFrom(src => src.Bar))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src.Metadata))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.BrandNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.Dimensions, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCategories, opt => opt.Ignore())
                .ForMember(dest => dest.ProductPrices, opt => opt.Ignore())
                .ForMember(dest => dest.ProductTrends, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UnitNavigation, opt => opt.Ignore());
            CreateMap<ProductTrend, GetListProductTrends>()
                .ForMember(dest => dest.Ptsid, opt => opt.MapFrom(src => src.Ptsid))
                .ForMember(dest => dest.Trend, opt => opt.MapFrom(src => src.Trend))
                .ForMember(dest => dest.Productid, opt => opt.MapFrom(src => src.Productid));
        }
    }
}