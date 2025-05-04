using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IProductStatusService
    {
        Task<ServiceResponse<IEnumerable<ProductStatus>>> GetAllProductStatus();
    }
}
