using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
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
    }
}
