﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Roles;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        
        /// <summary>
        /// Lấy danh sách tất cả các role trong hệ thống
        /// </summary>
        /// <returns>Danh sách các role</returns>
        public async Task<ServiceResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesAsync();
                return ServiceResponse<IEnumerable<RoleDTO>>.SuccessResponse(roles, "Lấy danh sách role thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<RoleDTO>>.ErrorResponse($"Lỗi khi lấy danh sách role: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Lấy thông tin chi tiết của một role theo ID
        /// </summary>
        /// <param name="roleId">ID của role</param>
        /// <returns>Thông tin chi tiết của role</returns>
        public async Task<ServiceResponse<RoleDTO>> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _roleRepository.GetRoleByIdAsync(roleId);
                if (role == null)
                    return ServiceResponse<RoleDTO>.NotFoundResponse("Không tìm thấy role");
                
                return ServiceResponse<RoleDTO>.SuccessResponse(role, "Lấy thông tin role thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<RoleDTO>.ErrorResponse($"Lỗi khi lấy thông tin role: {ex.Message}");
            }
        }
    }
}
