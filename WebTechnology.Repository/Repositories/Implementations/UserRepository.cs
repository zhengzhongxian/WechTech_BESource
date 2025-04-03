using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly WebTech _context;

        public UserRepository(WebTech context)
        {
            _context = context;
        }
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

            if (user == null) return null;

            if (!VerifyPassword(password, user.Password)) return null;

            return user;
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
    }
}
