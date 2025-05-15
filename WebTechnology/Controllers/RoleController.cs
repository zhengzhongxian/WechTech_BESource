﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        
        /// <summary>
        /// Lấy danh sách tất cả các role trong hệ thống
        /// </summary>
        /// <returns>Danh sách các role</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return StatusCode((int)response.StatusCode, response);
        }
        
        /// <summary>
        /// Lấy thông tin chi tiết của một role theo ID
        /// </summary>
        /// <param name="roleId">ID của role</param>
        /// <returns>Thông tin chi tiết của role</returns>
        [HttpGet("{roleId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var response = await _roleService.GetRoleByIdAsync(roleId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
