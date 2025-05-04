using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        private readonly WebTech _webTech;
        public ImageRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }

        public Task<ImageDTO?> GetImageByOrder(string order)
        {
            var image = _webTech.Images.Where(x => x.Order == order).Select(x => new ImageDTO
            {
                Imageid = x.Imageid,
                ImageData = x.ImageData,
                Order = x.Order,
                Publicid = x.Publicid
            }).FirstOrDefaultAsync();
            return image;
        }
    }
}
