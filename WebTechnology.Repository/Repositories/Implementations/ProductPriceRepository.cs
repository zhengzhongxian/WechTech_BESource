using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Products;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ProductPriceRepository : GenericRepository<ProductPrice>, IProductPriceRepository
    {
        private readonly WebTech _webTech;
        public ProductPriceRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }

        public async Task<ProductPriceDTO> GetProductPriceAsync(string productId)
        {
            var productPriceIsActive = await _webTech.ProductPrices.Where(x => x.Productid == productId && x.IsActive == true)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();
            var productPriceIsDefault = await _webTech.ProductPrices.Where(x => x.Productid == productId && x.IsDefault == true)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();
            return new ProductPriceDTO
            {
                PriceIsActive = productPriceIsActive.HasValue ? productPriceIsActive.Value : 0,
                PriceIsDefault = productPriceIsDefault.HasValue ? productPriceIsDefault.Value : 0,
            };
        }
    }
}
