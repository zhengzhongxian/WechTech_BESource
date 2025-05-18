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
    public interface IAuthService
    {
        Task<AuthResponse> AdminLoginAsync(string username, string password);
        Task<AuthResponse> CustomerLoginAsync(string username, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<ServiceResponse<string>> OTPAuthAsync(string email);
        Task<ServiceResponse<string>> RegisterAsync(RegistrationRequestDTO registrationRequest);
        Task<ServiceResponse<string>> LogoutAsync(string token);

        /// <summary>
        /// Kiểm tra xem email đã tồn tại chưa
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        Task<ServiceResponse<bool>> CheckEmailExistsAsync(string email);

        /// <summary>
        /// Kiểm tra xem username đã tồn tại chưa
        /// </summary>
        /// <param name="username">Username cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        Task<ServiceResponse<bool>> CheckUsernameExistsAsync(string username);

        /// <summary>
        /// Kiểm tra xem email và username đã tồn tại chưa
        /// </summary>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="username">Username cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        Task<ServiceResponse<bool>> CheckEmailAndUsernameExistAsync(string email, string username);
    }
}
