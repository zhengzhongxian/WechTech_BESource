using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.UserStatuses;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class UserStatusService : IUserStatusService
    {
        private readonly IUserStatusRepository _userStatusRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserStatusService> _logger;

        public UserStatusService(IUserStatusRepository userStatusRepository, IMapper mapper,
            IUnitOfWork unitOfWork, ILogger<UserStatusService> logger)
        {
            _userStatusRepository = userStatusRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserStatus>> CreateUserStatusAsync(CreateUserStatusDTO createDto)
        {
            try
            {
                var userStatus = _mapper.Map<UserStatus>(createDto);
                userStatus.StatusId = Guid.NewGuid().ToString();
                userStatus.CreatedAt = DateTime.UtcNow;
                await _userStatusRepository.AddAsync(userStatus);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<UserStatus>.SuccessResponse("Tạo trạng thái người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<UserStatus>.ErrorResponse($"Lỗi khi tạo trạng thái người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<UserStatus>>> GetUserStatusesAsync()
        {
            try
            {
                var userStatuses = await _userStatusRepository.GetAllAsync();
                return ServiceResponse<IEnumerable<UserStatus>>.SuccessResponse(userStatuses, "Lấy danh sách trạng thái người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<UserStatus>>.ErrorResponse($"Lỗi khi lấy danh sách trạng thái người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<UserStatus>> PatchUserStatusAsync(string id, JsonPatchDocument<UserStatus> patchDoc)
        {
            try
            {
                var exist = await _userStatusRepository.GetByIdAsync(id);
                if (exist == null)
                {
                    return ServiceResponse<UserStatus>.NotFoundResponse("Trạng thái người dùng không tồn tại");
                }
                patchDoc.ApplyTo(exist);
                exist.UpdatedAt = DateTime.UtcNow;
                await _userStatusRepository.UpdateAsync(exist);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<UserStatus>.SuccessResponse("Cập nhật trạng thái người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<UserStatus>.ErrorResponse($"Lỗi khi cập nhật trạng thái người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> DeleteUserStatusAsync(string id)
        {
            try
            {
                var exist = await _userStatusRepository.GetByIdAsync(id);
                if (exist == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Trạng thái người dùng không tồn tại");
                }
                await _userStatusRepository.DeleteAsync(exist);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Xóa trạng thái người dùng thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi xóa trạng thái người dùng: {ex.Message}");
            }
        }
    }
}