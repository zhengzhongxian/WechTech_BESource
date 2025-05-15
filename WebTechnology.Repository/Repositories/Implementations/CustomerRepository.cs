using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly WebTech _context;
        public CustomerRepository(WebTech context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng từ cả bảng User và Customer
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <returns>Thông tin chi tiết của khách hàng</returns>
        public async Task<CustomerDetailDTO> GetCustomerDetailAsync(string customerId)
        {
            return await _context.Customers
                .Include(c => c.CustomerNavigation) // Join với bảng User
                .Include(c => c.CustomerNavigation.Status) // Join với bảng UserStatus
                .Include(c => c.CustomerNavigation.Role) // Join với bảng Role
                .Where(c => c.Customerid == customerId)
                .Select(c => new CustomerDetailDTO
                {
                    // Thông tin từ bảng User
                    UserId = c.CustomerNavigation.Userid,
                    Username = c.CustomerNavigation.Username,
                    Email = c.CustomerNavigation.Email,
                    IsActive = c.CustomerNavigation.IsActive,
                    IsDeleted = c.CustomerNavigation.IsDeleted,
                    CreatedAt = c.CustomerNavigation.CreatedAt,
                    UpdatedAt = c.CustomerNavigation.UpdatedAt,
                    StatusId = c.CustomerNavigation.StatusId,
                    StatusName = c.CustomerNavigation.Status.Name,
                    RoleId = c.CustomerNavigation.Roleid,
                    RoleName = c.CustomerNavigation.Role.RoleName,

                    // Thông tin từ bảng Customer
                    CustomerId = c.Customerid,
                    Surname = c.Surname,
                    Middlename = c.Middlename,
                    Firstname = c.Firstname,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Avatar = c.Avatar,
                    AvatarPublicId = c.Publicid, // Lấy PublicId từ trường Publicid trong database
                    Dob = c.Dob,
                    Gender = c.Gender,
                    Coupoun = c.Coupoun
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng từ cả bảng User và Customer
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <returns>Kết quả cập nhật</returns>
        public async Task<bool> UpdateCustomerDetailAsync(string customerId, UpdateCustomerDetailDTO updateDto)
        {
            // Tìm thông tin khách hàng
            var customer = await _context.Customers
                .Include(c => c.CustomerNavigation)
                .FirstOrDefaultAsync(c => c.Customerid == customerId);

            if (customer == null)
                return false;

            // Cập nhật thông tin từ bảng Customer
            if (updateDto.Surname != null)
                customer.Surname = updateDto.Surname;

            if (updateDto.Middlename != null)
                customer.Middlename = updateDto.Middlename;

            if (updateDto.Firstname != null)
                customer.Firstname = updateDto.Firstname;

            if (updateDto.PhoneNumber != null)
                customer.PhoneNumber = updateDto.PhoneNumber;

            if (updateDto.Address != null)
                customer.Address = updateDto.Address;

            // Xử lý avatar - Đã được xử lý ở tầng Service với Cloudinary
            // Không cần xử lý ở đây nữa vì đã xử lý ở tầng Service

            if (updateDto.Dob.HasValue)
                customer.Dob = updateDto.Dob;

            if (updateDto.Gender != null)
                customer.Gender = updateDto.Gender;

            // Cập nhật thông tin từ bảng User
            var user = customer.CustomerNavigation;

            if (updateDto.Username != null)
                user.Username = updateDto.Username;

            if (updateDto.Email != null)
                user.Email = updateDto.Email;

            if (updateDto.IsActive.HasValue)
                user.IsActive = updateDto.IsActive;

            if (updateDto.StatusId != null)
                user.StatusId = updateDto.StatusId;

            if (updateDto.RoleId != null)
                user.Roleid = updateDto.RoleId;

            // Cập nhật thời gian
            user.UpdatedAt = DateTime.UtcNow;

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
