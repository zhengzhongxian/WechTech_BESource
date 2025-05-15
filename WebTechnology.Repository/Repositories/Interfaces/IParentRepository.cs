using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IParentRepository : IGenericRepository<Parent>
    {
        Task<IEnumerable<Parent>> GetAllParentsAsync();
        Task<Parent?> GetParentByIdAsync(string id);
        Task<bool> IsParentNameExistsAsync(string name, string excludeId = null);
    }
}
