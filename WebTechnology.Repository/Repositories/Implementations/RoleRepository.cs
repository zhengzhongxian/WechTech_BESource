﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Roles;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly WebTech _context;
        
        public RoleRepository(WebTech context) : base(context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Lấy danh sách tất cả các role trong hệ thống
        /// </summary>
        /// <returns>Danh sách các role</returns>
        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDTO
                {
                    RoleId = r.Roleid,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    Priority = r.Priority,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();
        }
        
        /// <summary>
        /// Lấy thông tin chi tiết của một role theo ID
        /// </summary>
        /// <param name="roleId">ID của role</param>
        /// <returns>Thông tin chi tiết của role</returns>
        public async Task<RoleDTO> GetRoleByIdAsync(string roleId)
        {
            return await _context.Roles
                .Where(r => r.Roleid == roleId)
                .Select(r => new RoleDTO
                {
                    RoleId = r.Roleid,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    Priority = r.Priority,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }
    }
}
