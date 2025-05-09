using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.Crud
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(object id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(object id);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetByPropertyAsync<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            TProperty value);
        Task<IEnumerable<TEntity>> GetByPropertyAsync<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            TProperty value,
            Expression<Func<TEntity, bool>> additionalFilter);
    }

}
