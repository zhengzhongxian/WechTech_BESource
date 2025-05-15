using WebTechnology.Repository.DTOs.Parents;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IParentService
    {
        Task<ServiceResponse<IEnumerable<ParentDTO>>> GetAllParentsAsync();
        Task<ServiceResponse<ParentDTO>> GetParentByIdAsync(string id);
        Task<ServiceResponse<ParentDTO>> CreateParentAsync(CreateParentDTO createParentDTO);
        Task<ServiceResponse<ParentDTO>> UpdateParentAsync(string id, CreateParentDTO updateParentDTO);
        Task<ServiceResponse<bool>> DeleteParentAsync(string id);
    }
}
