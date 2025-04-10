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
    public class ProductTrendsRepository : GenericRepository<ProductTrend>, IProductTrendsRepository
    {
        private readonly WebTech _context;
        public ProductTrendsRepository(WebTech context) : base(context)
        {
            _context = context;
        }
    }
}
