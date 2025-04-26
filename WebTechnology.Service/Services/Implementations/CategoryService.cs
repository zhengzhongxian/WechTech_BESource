using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System.Net;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Categories;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper,
            ILogger<CategoryService> logger,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<IEnumerable<CategoryDTO>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesWithParentAsync();
                var categoryDtos = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

                return new ServiceResponse<IEnumerable<CategoryDTO>>
                {
                    Data = categoryDtos,
                    Message = "Categories retrieved successfully",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return ServiceResponse<IEnumerable<CategoryDTO>>.ErrorResponse(
                    "Error retrieving categories",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<CategoryDTO>> GetCategoryByIdAsync(string id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithParentAsync(id);
                if (category == null)
                {
                    return ServiceResponse<CategoryDTO>.ErrorResponse(
                        "Category not found",
                        HttpStatusCode.NotFound);
                }

                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return new ServiceResponse<CategoryDTO>
                {
                    Data = categoryDto,
                    Message = "Category retrieved successfully",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {Id}", id);
                return ServiceResponse<CategoryDTO>.ErrorResponse(
                    "Error retrieving category",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<CategoryDTO>> CreateCategoryAsync(CreateCategoryDTO createDto)
        {
            try
            {
                if (await _categoryRepository.IsCategoryNameExistsAsync(createDto.CategoryName))
                {
                    return ServiceResponse<CategoryDTO>.ErrorResponse(
                        "Category name already exists",
                        HttpStatusCode.BadRequest);
                }

                var category = _mapper.Map<Category>(createDto);
                category.Categoryid = Guid.NewGuid().ToString();
                category.CreatedAt = DateTime.UtcNow;

                await _categoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return new ServiceResponse<CategoryDTO>
                {
                    Data = categoryDto,
                    Message = "Category created successfully",
                    StatusCode = HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return ServiceResponse<CategoryDTO>.ErrorResponse(
                    "Error creating category",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<CategoryDTO>> PatchCategoryAsync(string id, JsonPatchDocument<Category> patchDoc)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return ServiceResponse<CategoryDTO>.ErrorResponse(
                        "Category not found",
                        HttpStatusCode.NotFound);
                }

                patchDoc.ApplyTo(category);
                category.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                var categoryDto = _mapper.Map<CategoryDTO>(category);
                return new ServiceResponse<CategoryDTO>
                {
                    Data = categoryDto,
                    Message = "Category updated successfully",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {Id}", id);
                return ServiceResponse<CategoryDTO>.ErrorResponse(
                    "Error updating category",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryAsync(string id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return ServiceResponse<bool>.ErrorResponse(
                        "Category not found",
                        HttpStatusCode.NotFound);
                }

                await _categoryRepository.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Category deleted successfully",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {Id}", id);
                return ServiceResponse<bool>.ErrorResponse(
                    "Error deleting category",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
} 