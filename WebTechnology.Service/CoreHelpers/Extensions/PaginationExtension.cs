using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.CoreHelpers.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize) where T : class
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>(items, new PaginationMetadata(pageNumber, pageSize, count));
        }

        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                source = source.Where(predicate);
            }

            return await source.ToPaginatedListAsync(pageNumber, pageSize);
        }

        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T, TProperty>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            Expression<Func<T, IEnumerable<TProperty>>> collectionSelector,
            Expression<Func<TProperty, bool>> predicate) where T : class
        {
            if (collectionSelector != null && predicate != null)
            {
                source = source.Where(e => EF.Property<IEnumerable<TProperty>>(e, GetPropertyName(collectionSelector))
                    .Any(predicate.Compile()));
            }

            return await source.ToPaginatedListAsync(pageNumber, pageSize);
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
