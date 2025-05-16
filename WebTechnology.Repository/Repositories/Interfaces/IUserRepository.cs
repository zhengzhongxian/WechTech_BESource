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

        /// <summary>
        /// Lấy danh sách người dùng có vai trò là Admin và Staff có phân trang
        /// </summary>
        Task<(IEnumerable<AdminStaffDTO> Users, int TotalCount)> GetPaginatedAdminStaffUsersAsync(AdminStaffQueryRequest queryRequest);

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        Task<AdminStaffDTO> GetAdminStaffDetailAsync(string userId);

        /// <summary>
        /// Kiểm tra xem người dùng có phải là Admin hoặc Staff không
        /// </summary>
        Task<bool> IsAdminOrStaffAsync(string userId);
    }
}
