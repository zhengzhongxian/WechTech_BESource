using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Cart;

namespace WebTechnology.Repository.CoreHelpers.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartItemDTO, CartItem>().ReverseMap();
            CreateMap<CreateCartItemDTO, CartItem>().ReverseMap();
        }
    }
}
