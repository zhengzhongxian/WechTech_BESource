using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Users;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng từ cả bảng User và Customer
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <returns>Thông tin chi tiết của khách hàng</returns>
        Task<CustomerDetailDTO> GetCustomerDetailAsync(string customerId);

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng từ cả bảng User và Customer
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<bool> UpdateCustomerDetailAsync(string customerId, UpdateCustomerDetailDTO updateDto);
    }
}
