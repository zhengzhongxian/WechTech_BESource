using Microsoft.EntityFrameworkCore;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly WebTech _context;
        public CategoryRepository(WebTech context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithParentAsync()
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithParentAsync(string id)
        {
            return await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Categoryid == id);
        }

        public async Task<bool> IsCategoryNameExistsAsync(string name, string excludeId = null)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(excludeId))
            {
                query = query.Where(c => c.Categoryid != excludeId);
            }
            return await query.AnyAsync(c => c.CategoryName == name);
        }
    }
} 