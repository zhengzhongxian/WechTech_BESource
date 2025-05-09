using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Dimensions;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimensionController : ControllerBase
    {
        private readonly IDimensionService _dimensionService;
        public DimensionController(IDimensionService dimensionService)
        {
            _dimensionService = dimensionService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetDimensions(string productId)
        {
            var response = await _dimensionService.GetDimensionAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDimension([FromBody] CreateDimensionDTO createDto)
        {
            var response = await _dimensionService.CreateDimensionAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{dimensionId}")]
        public async Task<IActionResult> UpdateDimension(string dimensionId, [FromBody] JsonPatchDocument<Dimension> patchDoc)
        {
            var response = await _dimensionService.UpdateDimensionAsync(dimensionId, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{dimensionId}")]
        public async Task<IActionResult> DeleteDimension(string dimensionId)
        {
            var response = await _dimensionService.DeleteDimensionAsync(dimensionId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}