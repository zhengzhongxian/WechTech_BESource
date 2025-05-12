using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Users;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task UpdateRefreshTokenAsync(string userId, string? refreshToken);
        Task<User?> GetUserByEmailAsync(string email);
        Task<(IEnumerable<UserWithStatusDTO> Users, int TotalCount)> GetPaginatedUsersWithStatusAsync(UserQueryRequest queryRequest);
    }
}
