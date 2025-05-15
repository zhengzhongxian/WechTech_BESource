using Microsoft.EntityFrameworkCore;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ParentRepository : GenericRepository<Parent>, IParentRepository
    {
        private readonly WebTech _context;
        
        public ParentRepository(WebTech context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Parent>> GetAllParentsAsync()
        {
            return await _context.Parents
                .OrderBy(p => p.Priority)
                .ToListAsync();
        }

        public async Task<Parent?> GetParentByIdAsync(string id)
        {
            return await _context.Parents
                .FirstOrDefaultAsync(p => p.Parentid == id);
        }

        public async Task<bool> IsParentNameExistsAsync(string name, string excludeId = null)
        {
            if (string.IsNullOrEmpty(excludeId))
            {
                return await _context.Parents.AnyAsync(p => p.ParentName == name);
            }
            else
            {
                return await _context.Parents.AnyAsync(p => p.ParentName == name && p.Parentid != excludeId);
            }
        }
    }
}
