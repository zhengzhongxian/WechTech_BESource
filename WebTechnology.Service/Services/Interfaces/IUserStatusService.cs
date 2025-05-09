using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.UserStatuses;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IUserStatusService
    {
        Task<ServiceResponse<UserStatus>> CreateUserStatusAsync(CreateUserStatusDTO createDto);
        Task<ServiceResponse<IEnumerable<UserStatus>>> GetUserStatusesAsync();
        Task<ServiceResponse<UserStatus>> PatchUserStatusAsync(string id, JsonPatchDocument<UserStatus> patchDoc);
        Task<ServiceResponse<string>> DeleteUserStatusAsync(string id);
    }
}