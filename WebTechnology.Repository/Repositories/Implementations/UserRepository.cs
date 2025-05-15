using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly WebTech _context;

        public UserRepository(WebTech context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Username == username
                            && u.IsActive == true
                            && u.IsDeleted == false
                            && u.Authenticate == true)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            if (!VerifyPassword(password, user.Password)) return null;

            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task UpdateRefreshTokenAsync(string userId, string? refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        private bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        public async Task<(IEnumerable<UserWithStatusDTO> Users, int TotalCount)> GetPaginatedUsersWithStatusAsync(UserQueryRequest queryRequest)
        {
            // Tạo truy vấn cơ bản - chỉ lấy người dùng có vai trò Customer (có bản ghi trong bảng Customer)
            var query = _context.Users
                .Include(u => u.Status)
                .Include(u => u.Customer)
                .Where(u => u.Customer != null && u.Roleid == RoleType.Customer.ToRoleIdString()) // Chỉ lấy người dùng là khách hàng
                .AsQueryable();

            // Áp dụng các bộ lọc
            if (!string.IsNullOrWhiteSpace(queryRequest.SearchTerm))
            {
                query = query.Where(u =>
                    (u.Username != null && u.Username.Contains(queryRequest.SearchTerm)) ||
                    (u.Email != null && u.Email.Contains(queryRequest.SearchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(queryRequest.StatusId))
            {
                query = query.Where(u => u.StatusId == queryRequest.StatusId);
            }

            if (queryRequest.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == queryRequest.IsActive.Value);
            }

            // Đếm tổng số bản ghi
            var totalCount = await query.CountAsync();

            // Áp dụng sắp xếp
            query = ApplySorting(query, queryRequest.SortBy, queryRequest.SortAscending);

            // Áp dụng phân trang
            var pagedUsers = await query
                .Skip((queryRequest.PageNumber - 1) * queryRequest.PageSize)
                .Take(queryRequest.PageSize)
                .Select(u => new UserWithStatusDTO
                {
                    UserId = u.Userid,
                    Username = u.Username,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    IsDeleted = u.IsDeleted,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    StatusId = u.StatusId,
                    StatusName = u.Status != null ? u.Status.Name : null
                })
                .ToListAsync();

            return (pagedUsers, totalCount);
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool sortAscending)
        {
            switch (sortBy?.ToLower())
            {
                case "username":
                    return sortAscending
                        ? query.OrderBy(u => u.Username)
                        : query.OrderByDescending(u => u.Username);
                case "email":
                    return sortAscending
                        ? query.OrderBy(u => u.Email)
                        : query.OrderByDescending(u => u.Email);
                case "createdat":
                default:
                    return sortAscending
                        ? query.OrderBy(u => u.CreatedAt)
                        : query.OrderByDescending(u => u.CreatedAt);
            }
        }
    }
}
