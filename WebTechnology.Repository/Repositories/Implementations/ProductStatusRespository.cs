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
    public class ProductStatusRespository : GenericRepository<ProductStatus>, IProductStatusRepository
    {
        private readonly WebTech _webTech;
        public ProductStatusRespository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }
    }
}
