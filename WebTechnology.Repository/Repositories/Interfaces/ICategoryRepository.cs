using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithParentAsync();
        Task<Category?> GetCategoryWithParentAsync(string id);
        Task<bool> IsCategoryNameExistsAsync(string name, string excludeId = null);
    }
} 