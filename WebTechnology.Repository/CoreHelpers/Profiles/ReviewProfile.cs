using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Review;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            // Map from Review to ReviewDTO
            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Reviewid))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customerid))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Productid))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            // Map from Comment to CommentDTO
            CreateMap<Comment, CommentDTO>()
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.Commentid))
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Reviewid))
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.CommentedAt, opt => opt.MapFrom(src => src.CommentedAt))
                .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(src => src.ModifiedAt));

            // Map from CreateReviewDTO to Review
            CreateMap<CreateReviewDTO, Review>()
                .ForMember(dest => dest.Productid, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.Reviewid, opt => opt.Ignore())
                .ForMember(dest => dest.Customerid, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // Map from CreateCommentDTO to Comment
            CreateMap<CreateCommentDTO, Comment>()
                .ForMember(dest => dest.Reviewid, opt => opt.MapFrom(src => src.ReviewId))
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.Commentid, opt => opt.Ignore())
                .ForMember(dest => dest.CommentedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Review, opt => opt.Ignore());
        }
    }
}
