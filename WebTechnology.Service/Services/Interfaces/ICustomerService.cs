using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ServiceResponse<CustomerDTO>> GetCustomerInfo(string token);
        Task<ServiceResponse<string>> UpdateCustomerInfo(string token, JsonPatchDocument<Customer> patchDoc);
        Task<ServiceResponse<PaginatedResult<UserWithStatusDTO>>> GetPaginatedUsersAsync(UserQueryRequest queryRequest);

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Thông tin chi tiết của khách hàng</returns>
        Task<ServiceResponse<CustomerDetailDTO>> GetCustomerDetailAsync(string customerId, string token);

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResponse<bool>> UpdateCustomerDetailAsync(string customerId, UpdateCustomerDetailDTO updateDto, string token);
    }
}
