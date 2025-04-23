using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Products;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IProductPriceRepository : IGenericRepository<ProductPrice>
    {
       Task<ProductPriceDTO> GetProductPriceAsync(string productId); 
    }
}
