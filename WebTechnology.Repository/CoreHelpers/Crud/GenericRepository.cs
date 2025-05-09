using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.Crud
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
                _dbSet.Attach(entity);

            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await _dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetByPropertyAsync<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            TProperty value)
        {
            // Tạo biểu thức so sánh property == value
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, GetPropertyName(propertySelector));
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            return await _dbSet.Where(lambda).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetByPropertyAsync<TProperty>(
            Expression<Func<TEntity, TProperty>> propertySelector,
            TProperty value,
            Expression<Func<TEntity, bool>> additionalFilter)
        {
            // Tạo biểu thức so sánh property == value
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, GetPropertyName(propertySelector));
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);

            // Kết hợp với additionalFilter
            return await _dbSet.Where(lambda).Where(additionalFilter).ToListAsync();
        }

        private static string GetPropertyName(LambdaExpression expression)
        {
            return expression.Body switch
            {
                MemberExpression m => m.Member.Name,
                UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
                _ => throw new ArgumentException("Expression is not a property access")
            };
        }
    }

}
