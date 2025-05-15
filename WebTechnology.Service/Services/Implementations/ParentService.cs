using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Net;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Parents;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.CoreHelpers.Extensions;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ParentService : IParentService
    {
        private readonly IParentRepository _parentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ParentService> _logger;

        public ParentService(IParentRepository parentRepository, IMapper mapper, ILogger<ParentService> logger)
        {
            _parentRepository = parentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<ParentDTO>>> GetAllParentsAsync()
        {
            try
            {
                var parents = await _parentRepository.GetAllParentsAsync();
                var parentDTOs = _mapper.Map<IEnumerable<ParentDTO>>(parents);

                return new ServiceResponse<IEnumerable<ParentDTO>>
                {
                    Data = parentDTOs,
                    Message = "Lấy danh sách danh mục cha thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách danh mục cha");
                return new ServiceResponse<IEnumerable<ParentDTO>>
                {
                    Message = "Lỗi khi lấy danh sách danh mục cha",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<ParentDTO>> GetParentByIdAsync(string id)
        {
            try
            {
                var parent = await _parentRepository.GetParentByIdAsync(id);
                if (parent == null)
                {
                    return new ServiceResponse<ParentDTO>
                    {
                        Message = "Không tìm thấy danh mục cha",
                        Success = false,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var parentDTO = _mapper.Map<ParentDTO>(parent);
                return new ServiceResponse<ParentDTO>
                {
                    Data = parentDTO,
                    Message = "Lấy thông tin danh mục cha thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin danh mục cha với ID: {Id}", id);
                return new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi khi lấy thông tin danh mục cha",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<ParentDTO>> CreateParentAsync(CreateParentDTO createParentDTO)
        {
            try
            {
                // Kiểm tra tên danh mục cha đã tồn tại chưa
                var isNameExists = await _parentRepository.IsParentNameExistsAsync(createParentDTO.ParentName);
                if (isNameExists)
                {
                    return new ServiceResponse<ParentDTO>
                    {
                        Message = "Tên danh mục cha đã tồn tại",
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                // Tạo mới danh mục cha
                var parent = _mapper.Map<Parent>(createParentDTO);
                parent.Parentid = Guid.NewGuid().ToString();
                parent.CreatedAt = DateTime.UtcNow;
                parent.UpdatedAt = DateTime.UtcNow;

                await _parentRepository.AddAsync(parent);
                var parentDTO = _mapper.Map<ParentDTO>(parent);

                return new ServiceResponse<ParentDTO>
                {
                    Data = parentDTO,
                    Message = "Tạo danh mục cha thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo danh mục cha");
                return new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi khi tạo danh mục cha",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<ParentDTO>> UpdateParentAsync(string id, CreateParentDTO updateParentDTO)
        {
            try
            {
                var parent = await _parentRepository.GetParentByIdAsync(id);
                if (parent == null)
                {
                    return new ServiceResponse<ParentDTO>
                    {
                        Message = "Không tìm thấy danh mục cha",
                        Success = false,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                // Kiểm tra tên danh mục cha đã tồn tại chưa (trừ chính nó)
                var isNameExists = await _parentRepository.IsParentNameExistsAsync(updateParentDTO.ParentName, id);
                if (isNameExists)
                {
                    return new ServiceResponse<ParentDTO>
                    {
                        Message = "Tên danh mục cha đã tồn tại",
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }

                // Cập nhật thông tin
                parent.ParentName = updateParentDTO.ParentName;
                parent.Priority = updateParentDTO.Priority;
                parent.UpdatedAt = DateTime.UtcNow;

                await _parentRepository.UpdateAsync(parent);
                var parentDTO = _mapper.Map<ParentDTO>(parent);

                return new ServiceResponse<ParentDTO>
                {
                    Data = parentDTO,
                    Message = "Cập nhật danh mục cha thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật danh mục cha với ID: {Id}", id);
                return new ServiceResponse<ParentDTO>
                {
                    Message = "Lỗi khi cập nhật danh mục cha",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteParentAsync(string id)
        {
            try
            {
                var parent = await _parentRepository.GetParentByIdAsync(id);
                if (parent == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Message = "Không tìm thấy danh mục cha",
                        Success = false,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                await _parentRepository.DeleteAsync(parent);

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Message = "Xóa danh mục cha thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa danh mục cha với ID: {Id}", id);
                return new ServiceResponse<bool>
                {
                    Message = "Lỗi khi xóa danh mục cha",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
