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

        [HttpPatch("{productId}")]
        public async Task<IActionResult> UpdateDimension(string productId, [FromBody] JsonPatchDocument<Dimension> patchDoc)
        {
            var response = await _dimensionService.UpdateDimensionAsync(productId, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteDimension(string productId)
        {
            var response = await _dimensionService.DeleteDimensionAsync(productId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}