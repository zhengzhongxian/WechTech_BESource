using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> AdminLoginAsync(string username, string password);
        Task<AuthResponse> CustomerLoginAsync(string username, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<ServiceResponse<string>> OTPAuthAsync(string email);
        Task<ServiceResponse<string>> RegisterAsync(RegistrationRequestDTO registrationRequest);
    }
}
