using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Trends;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ITrendService
    {
        Task<ServiceResponse<Trend>> CreateTrendAsync(CreateTrendDTO createDto);
        Task<ServiceResponse<IEnumerable<Trend>>> GetTrendsAsync();
        Task<ServiceResponse<Trend>> PatchTrendAsync(string id, JsonPatchDocument<Trend> patchDoc);
        Task<ServiceResponse<string>> DeleteTrendAsync(string id);
    }
}
