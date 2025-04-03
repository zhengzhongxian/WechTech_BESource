using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementationns
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }
        public async Task<AuthResponse> AdminLoginAsync(string username, string password)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.AuthenticateAsync(username, password);

                if (user == null)
                    return AuthResponse.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                if (user.Role?.RoleName != "Admin")
                    return AuthResponse.Fail("Access denied. Admin only.", HttpStatusCode.Forbidden);

                var result = await GenerateAuthResponse(user);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return AuthResponse.Error($"{ex.Message}");
            }
        }

        public async Task<AuthResponse> CustomerLoginAsync(string username, string password)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.AuthenticateAsync(username, password);

                if (user == null)
                    return AuthResponse.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                if (user.Role?.RoleName != "Customer")
                    return AuthResponse.Fail("Quyền truy cập bị từ chối. Chỉ dành cho quản trị viên.", HttpStatusCode.Forbidden);

                var result = await GenerateAuthResponse(user);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return AuthResponse.Error($"{ex.Message}");
            }
        }

        public Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            throw new NotImplementedException();
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _userRepository.UpdateRefreshTokenAsync(user.Userid, refreshToken);
            return AuthResponse.LoginSuccess(accessToken, refreshToken);
        }
    }
}
