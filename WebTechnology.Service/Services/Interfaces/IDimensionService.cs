using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IDimensionService
    {
        Task<ServiceResponse<IEnumerable<Dimension>>> GetDimensionAsync(string productId);
        Task<ServiceResponse<Dimension>> CreateDimensionAsync(CreateDimensionDTO createDto);
        Task<ServiceResponse<Dimension>> UpdateDimensionAsync(string dimensionId, JsonPatchDocument<Dimension> patchDoc);
        Task<ServiceResponse<bool>> DeleteDimensionAsync(string dimensionId);
    }
}
