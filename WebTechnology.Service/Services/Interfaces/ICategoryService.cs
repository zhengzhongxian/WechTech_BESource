using Microsoft.AspNetCore.JsonPatch;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Categories;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetCategoriesAsync();
        Task<ServiceResponse<CategoryDTO>> GetCategoryByIdAsync(string id);
        Task<ServiceResponse<CategoryDTO>> CreateCategoryAsync(CreateCategoryDTO createDto);
        Task<ServiceResponse<CategoryDTO>> PatchCategoryAsync(string id, JsonPatchDocument<Category> patchDoc);
        Task<ServiceResponse<bool>> DeleteCategoryAsync(string id);
    }
} 