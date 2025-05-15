using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Categories;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>List of categories</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _categoryService.GetCategoriesAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="createDto">Category creation data</param>
        /// <returns>Created category</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResponse<Category>.ErrorResponse(
                    "Invalid data",
                    System.Net.HttpStatusCode.BadRequest,
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            }

            var response = await _categoryService.CreateCategoryAsync(createDto);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Update a category using PATCH
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <param name="patchDoc">PATCH document</param>
        /// <returns>Updated category</returns>
        [HttpPatch("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> PatchCategory(string id, [FromBody] JsonPatchDocument<Category> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(ServiceResponse<Category>.ErrorResponse(
                    "Invalid patch document",
                    System.Net.HttpStatusCode.BadRequest));
            }

            var response = await _categoryService.PatchCategoryAsync(id, patchDoc);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrStaff")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
} 