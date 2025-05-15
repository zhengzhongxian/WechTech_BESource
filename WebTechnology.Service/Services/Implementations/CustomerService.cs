using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WebTechnology.Service.Services.Implementationns
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;

        public CustomerService(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ServiceResponse<CustomerDTO>> GetCustomerInfo(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Token không hợp lệ");
                }
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Token đã hết hạn");
                }

                var userId = _tokenService.GetUserIdFromToken(token);

                if (userId == null)
                {
                    return ServiceResponse<CustomerDTO>.FailResponse("Không tìm thấy thông tin người dùng");
                }
                var customer = await _customerRepository.GetByIdAsync(userId);
                if (customer == null)
                {
                    return ServiceResponse<CustomerDTO>.NotFoundResponse("Không tìm thấy thông tin người dùng");
                }
                var customerDto = _mapper.Map<CustomerDTO>(customer);
                var email = await _userRepository.GetByIdAsync(userId);
                customerDto.Email = email.Email ?? "Khong co email";
                return ServiceResponse<CustomerDTO>.SuccessResponse(customerDto, "Lấy thông tin người dùng thành công");

            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerDTO>.ErrorResponse($"Có lỗi phía server nhé FE {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateCustomerInfo(string token, JsonPatchDocument<Customer> patchDoc)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return ServiceResponse<string>.FailResponse("Token không hợp lệ");
                }
                if (_tokenService.IsTokenExpired(token))
                {
                    return ServiceResponse<string>.FailResponse("Token đã hết hạn");
                }
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                {
                    return ServiceResponse<string>.FailResponse("Không tìm thấy thông tin người dùng");
                }
                var customer = await _customerRepository.GetByIdAsync(userId);
                if (customer == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Không tìm thấy thông tin người dùng");
                }
                patchDoc.ApplyTo(customer);
                await _customerRepository.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Cập nhật thông tin người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Có lỗi phía server nhé FE {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PaginatedResult<UserWithStatusDTO>>> GetPaginatedUsersAsync(UserQueryRequest queryRequest)
        {
            try
            {
                var (users, totalCount) = await _userRepository.GetPaginatedUsersWithStatusAsync(queryRequest);

                var paginationMetadata = new PaginationMetadata(
                    queryRequest.PageNumber,
                    queryRequest.PageSize,
                    totalCount
                );

                var result = new PaginatedResult<UserWithStatusDTO>(
                    users.ToList(),
                    paginationMetadata
                );

                return ServiceResponse<PaginatedResult<UserWithStatusDTO>>.SuccessResponse(
                    result,
                    "Lấy danh sách người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<PaginatedResult<UserWithStatusDTO>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Thông tin chi tiết của khách hàng</returns>
        public async Task<ServiceResponse<CustomerDetailDTO>> GetCustomerDetailAsync(string customerId, string token)
        {
            try
            {
                // Kiểm tra token
                if (string.IsNullOrEmpty(token))
                    return ServiceResponse<CustomerDetailDTO>.FailResponse("Token không hợp lệ");

                if (_tokenService.IsTokenExpired(token))
                    return ServiceResponse<CustomerDetailDTO>.FailResponse("Token đã hết hạn");

                // Lấy thông tin người dùng từ token
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                    return ServiceResponse<CustomerDetailDTO>.FailResponse("Không tìm thấy thông tin người dùng");

                // Kiểm tra quyền Admin
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.Roleid != RoleType.Admin.ToRoleIdString())
                    return ServiceResponse<CustomerDetailDTO>.FailResponse("Bạn không có quyền truy cập thông tin này", System.Net.HttpStatusCode.Forbidden);

                // Lấy thông tin chi tiết của khách hàng
                var customerDetail = await _customerRepository.GetCustomerDetailAsync(customerId);
                if (customerDetail == null)
                    return ServiceResponse<CustomerDetailDTO>.NotFoundResponse("Không tìm thấy thông tin khách hàng");

                return ServiceResponse<CustomerDetailDTO>.SuccessResponse(customerDetail, "Lấy thông tin chi tiết khách hàng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<CustomerDetailDTO>.ErrorResponse($"Lỗi khi lấy thông tin chi tiết khách hàng: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng từ cả bảng User và Customer (chỉ dành cho Admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="updateDto">Thông tin cần cập nhật</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Kết quả cập nhật</returns>
        public async Task<ServiceResponse<bool>> UpdateCustomerDetailAsync(string customerId, UpdateCustomerDetailDTO updateDto, string token)
        {
            try
            {
                // Kiểm tra token
                if (string.IsNullOrEmpty(token))
                    return ServiceResponse<bool>.FailResponse("Token không hợp lệ");

                if (_tokenService.IsTokenExpired(token))
                    return ServiceResponse<bool>.FailResponse("Token đã hết hạn");

                // Lấy thông tin người dùng từ token
                var userId = _tokenService.GetUserIdFromToken(token);
                if (userId == null)
                    return ServiceResponse<bool>.FailResponse("Không tìm thấy thông tin người dùng");

                // Kiểm tra quyền Admin
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.Roleid != RoleType.Admin.ToRoleIdString())
                    return ServiceResponse<bool>.FailResponse("Bạn không có quyền cập nhật thông tin này", System.Net.HttpStatusCode.Forbidden);

                // Bắt đầu transaction
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Xử lý ảnh đại diện nếu có
                    // Chỉ xử lý khi AvatarBase64 không phải là null và không phải là chuỗi rỗng
                    if (!string.IsNullOrEmpty(updateDto.AvatarBase64) && updateDto.AvatarBase64 != string.Empty)
                    {
                        // Lấy thông tin khách hàng hiện tại để kiểm tra ảnh cũ
                        var customerDetail = await _customerRepository.GetCustomerDetailAsync(customerId);

                        // Xóa ảnh cũ trên Cloudinary nếu có
                        if (customerDetail != null && !string.IsNullOrEmpty(customerDetail.AvatarPublicId))
                        {
                            try
                            {
                                await _cloudinaryService.DeleteImageAsync(customerDetail.AvatarPublicId);
                            }
                            catch (Exception ex)
                            {
                                // Ghi log lỗi nhưng không dừng quá trình cập nhật
                                Console.WriteLine($"Lỗi khi xóa ảnh cũ: {ex.Message}");
                            }
                        }

                        // Upload ảnh mới lên Cloudinary
                        var uploadResult = await _cloudinaryService.UploadImageAsync(updateDto.AvatarBase64, "Customer");
                        if (uploadResult.Error != null)
                        {
                            return ServiceResponse<bool>.ErrorResponse($"Lỗi khi upload ảnh: {uploadResult.Error.Message}");
                        }

                        // Tạo đối tượng mới để lưu thông tin ảnh
                        var customerToUpdate = await _customerRepository.GetByIdAsync(customerId);
                        if (customerToUpdate != null)
                        {
                            customerToUpdate.Avatar = uploadResult.SecureUrl.ToString();
                            customerToUpdate.Publicid = uploadResult.PublicId;
                            await _customerRepository.UpdateAsync(customerToUpdate);
                        }
                    }

                    // Cập nhật thông tin chi tiết của khách hàng
                    var result = await _customerRepository.UpdateCustomerDetailAsync(customerId, updateDto);
                    if (!result)
                        return ServiceResponse<bool>.NotFoundResponse("Không tìm thấy thông tin khách hàng");

                    // Commit transaction
                    await _unitOfWork.CommitAsync();

                    return ServiceResponse<bool>.SuccessResponse(true, "Cập nhật thông tin chi tiết khách hàng thành công");
                }
                catch (Exception ex)
                {
                    // Rollback transaction nếu có lỗi
                    await _unitOfWork.RollbackAsync();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi cập nhật thông tin chi tiết khách hàng: {ex.Message}");
            }
        }
    }
}
