﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Roles;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// Lấy danh sách tất cả các role trong hệ thống
        /// </summary>
        /// <returns>Danh sách các role</returns>
        Task<ServiceResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync();
        
        /// <summary>
        /// Lấy thông tin chi tiết của một role theo ID
        /// </summary>
        /// <param name="roleId">ID của role</param>
        /// <returns>Thông tin chi tiết của role</returns>
        Task<ServiceResponse<RoleDTO>> GetRoleByIdAsync(string roleId);
    }
}
