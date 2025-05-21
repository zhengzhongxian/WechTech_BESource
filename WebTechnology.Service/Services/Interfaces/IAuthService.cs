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
        /// Đổi mật khẩu cho người dùng đã đăng nhập
        /// </summary>
        /// <param name="token">Token xác thực của người dùng</param>
        /// <param name="changePasswordDTO">Thông tin mật khẩu cũ và mới</param>
        /// <returns>Kết quả thay đổi mật khẩu</returns>
        Task<ServiceResponse<string>> ChangePasswordAsync(string token, ChangePasswordDTO changePasswordDTO);

        /// <summary>
        /// Gửi email đặt lại mật khẩu cho người dùng quên mật khẩu
        /// </summary>
        /// <param name="forgotPasswordDTO">Thông tin email</param>
        /// <param name="resetUrl">URL trang đặt lại mật khẩu</param>
        /// <returns>Kết quả gửi email</returns>
        Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO, string resetUrl);

        /// <summary>
        /// Đặt lại mật khẩu cho người dùng quên mật khẩu
        /// </summary>
        /// <param name="resetPasswordDTO">Thông tin mật khẩu mới và token</param>
        /// <returns>Kết quả đặt lại mật khẩu</returns>
        Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);

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
