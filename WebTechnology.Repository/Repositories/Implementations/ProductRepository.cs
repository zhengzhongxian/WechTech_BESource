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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly WebTech _context;
        public ProductRepository(WebTech context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Product> GetProductAsQueryable()
        {
            return _context.Products
                   .Where(p => p.IsActive == true && p.IsDeleted != true)
                   .AsQueryable();
        }
    }
}
