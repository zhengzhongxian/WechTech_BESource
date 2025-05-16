﻿using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IAdminService
    {
        /// <summary>
        /// Lấy danh sách người dùng có vai trò là Admin và Staff có phân trang
        /// </summary>
        /// <param name="queryRequest">Thông tin phân trang và tìm kiếm</param>
        /// <returns>Danh sách người dùng có phân trang</returns>
        Task<ServiceResponse<PaginatedResult<AdminStaffDTO>>> GetPaginatedAdminStaffUsersAsync(AdminStaffQueryRequest queryRequest);

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Thông tin chi tiết của người dùng</returns>
        Task<ServiceResponse<AdminStaffDTO>> GetAdminStaffDetailAsync(string userId);

        /// <summary>
        /// Cập nhật thông tin của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResponse<string>> UpdateAdminStaffAsync(string userId, UpdateAdminStaffDTO updateDto);

        /// <summary>
        /// Cập nhật toàn bộ thông tin của một khách hàng (bao gồm cả coupon và mật khẩu)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResponse<string>> UpdateCustomerFullAsync(string customerId, UpdateCustomerFullDTO updateDto);
    }
}
