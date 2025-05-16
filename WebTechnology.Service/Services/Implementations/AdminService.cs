﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Models.Pagination;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Multimedia;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public AdminService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ServiceResponse<PaginatedResult<AdminStaffDTO>>> GetPaginatedAdminStaffUsersAsync(AdminStaffQueryRequest queryRequest)
        {
            try
            {
                var (users, totalCount) = await _userRepository.GetPaginatedAdminStaffUsersAsync(queryRequest);

                var paginationMetadata = new PaginationMetadata(
                    queryRequest.PageNumber,
                    queryRequest.PageSize,
                    totalCount
                );

                var result = new PaginatedResult<AdminStaffDTO>(
                    users.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<AdminStaffDTO>>.SuccessResponse(
                    result,
                    "Lấy danh sách người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<AdminStaffDTO>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<AdminStaffDTO>> GetAdminStaffDetailAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetAdminStaffDetailAsync(userId);
                if (user == null)
                {
                    return ServiceResponse<AdminStaffDTO>.NotFoundResponse("Không tìm thấy người dùng");
                }

                return ServiceResponse<AdminStaffDTO>.SuccessResponse(user, "Lấy thông tin người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<AdminStaffDTO>.ErrorResponse($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateAdminStaffAsync(string userId, UpdateAdminStaffDTO updateDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra người dùng có tồn tại không
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy người dùng");
                }

                // Kiểm tra người dùng có phải là Admin hoặc Staff không
                if (user.Roleid != RoleType.Admin.ToRoleIdString() && user.Roleid != RoleType.Staff.ToRoleIdString())
                {
                    return ServiceResponse<string>.FailResponse("Người dùng không phải là Admin hoặc Staff");
                }

                // Cập nhật thông tin cơ bản
                user.Username = updateDto.Username;
                user.Email = updateDto.Email;

                // Cập nhật mật khẩu nếu có
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                // Cập nhật trạng thái
                user.IsActive = updateDto.IsActive;
                if (!string.IsNullOrEmpty(updateDto.StatusId))
                {
                    user.StatusId = updateDto.StatusId;
                }

                // Cập nhật vai trò nếu có
                if (!string.IsNullOrEmpty(updateDto.RoleId))
                {
                    // Chỉ cho phép cập nhật vai trò giữa Admin và Staff
                    if (updateDto.RoleId == RoleType.Admin.ToRoleIdString() || updateDto.RoleId == RoleType.Staff.ToRoleIdString())
                    {
                        user.Roleid = updateDto.RoleId;
                    }
                    else
                    {
                        return ServiceResponse<string>.FailResponse("Vai trò không hợp lệ");
                    }
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return ServiceResponse<string>.SuccessResponse("Cập nhật thông tin người dùng thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi cập nhật thông tin người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateCustomerFullAsync(string customerId, UpdateCustomerFullDTO updateDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra khách hàng có tồn tại không
                var user = await _userRepository.GetByIdAsync(customerId);
                if (user == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy người dùng");
                }

                // Kiểm tra người dùng có phải là Customer không
                if (user.Roleid != RoleType.Customer.ToRoleIdString())
                {
                    return ServiceResponse<string>.FailResponse("Người dùng không phải là khách hàng");
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy thông tin khách hàng");
                }

                // Cập nhật thông tin User
                user.Username = updateDto.Username;
                user.Email = updateDto.Email;

                // Cập nhật mật khẩu nếu có
                if (!string.IsNullOrEmpty(updateDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }

                // Cập nhật trạng thái
                user.IsActive = updateDto.IsActive;
                if (!string.IsNullOrEmpty(updateDto.StatusId))
                {
                    user.StatusId = updateDto.StatusId;
                }

                user.UpdatedAt = DateTime.UtcNow;

                // Cập nhật thông tin Customer
                customer.Surname = updateDto.Surname ?? customer.Surname;
                customer.Middlename = updateDto.Middlename ?? customer.Middlename;
                customer.Firstname = updateDto.Firstname ?? customer.Firstname;
                customer.PhoneNumber = updateDto.PhoneNumber ?? customer.PhoneNumber;
                customer.Address = updateDto.Address ?? customer.Address;
                customer.Dob = updateDto.Dob ?? customer.Dob;
                customer.Gender = updateDto.Gender ?? customer.Gender;

                // Cập nhật Coupon
                if (updateDto.Coupoun.HasValue)
                {
                    customer.Coupoun = updateDto.Coupoun.Value;
                }

                // Xử lý Avatar base64 nếu có
                if (!string.IsNullOrEmpty(updateDto.AvatarBase64))
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(customer.Publicid))
                    {
                        await _cloudinaryService.DeleteImageAsync(customer.Publicid);
                    }

                    // Upload ảnh mới lên Cloudinary
                    var uploadResult = await _cloudinaryService.UploadImageAsync(updateDto.AvatarBase64, "Customer");

                    // Cập nhật thông tin ảnh
                    customer.Avatar = uploadResult.SecureUrl.ToString();
                    customer.Publicid = uploadResult.PublicId;
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return ServiceResponse<string>.SuccessResponse("Cập nhật thông tin khách hàng thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi cập nhật thông tin khách hàng: {ex.Message}");
            }
        }
    }
}
