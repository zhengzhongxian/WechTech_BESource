﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Roles;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        /// <summary>
        /// Lấy danh sách tất cả các role trong hệ thống
        /// </summary>
        /// <returns>Danh sách các role</returns>
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        
        /// <summary>
        /// Lấy thông tin chi tiết của một role theo ID
        /// </summary>
        /// <param name="roleId">ID của role</param>
        /// <returns>Thông tin chi tiết của role</returns>
        Task<RoleDTO> GetRoleByIdAsync(string roleId);
    }
}
