using System;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.SeedData
{
    public static class AdminSeedData
    {
        public static async Task SeedAdminUserAsync(WebTech context, IUserRepository userRepository)
        {
            // Check if admin user already exists
            var existingAdmin = await userRepository.GetUserByEmailAsync("admin@webt.com");
            if (existingAdmin != null) return;

            // Create new admin user
            var adminUser = new User
            {
                Userid = Guid.NewGuid().ToString(),
                Username = "admin",
                Email = "admin@webt.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Default password
                Roleid = RoleType.Admin.ToRoleIdString(),
                Authenticate = true,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
} 