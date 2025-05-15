using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Generations;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace WebTechnology.Service.Services.Implementationns
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IMapper mapper,
            ICustomerRepository customerRepository,
            ICartRepository cartRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _cartRepository = cartRepository;
        }
        public async Task<AuthResponse> AdminLoginAsync(string username, string password)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _userRepository.AuthenticateAsync(username, password);

                if (user == null)
                    return AuthResponse.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // Kiểm tra xem người dùng có phải là Admin hoặc Staff không
                if (user.Roleid != RoleType.Admin.ToRoleIdString() && user.Roleid != RoleType.Staff.ToRoleIdString())
                    return AuthResponse.Fail("Quyền truy cập bị từ chối. Chỉ dành cho Admin hoặc Staff!", HttpStatusCode.Forbidden);
                if (user.StatusId == UserStatusType.Banned.ToUserStatusIdString())
                    return AuthResponse.Fail("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên để biết thêm thông tin.", HttpStatusCode.Forbidden);
                user.StatusId = UserStatusType.Active.ToUserStatusIdString();
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
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

        public async Task<AuthResponse> CustomerLoginAsync(string username, string password)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _userRepository.AuthenticateAsync(username, password);

                if (user == null)
                    return AuthResponse.Fail("Tên đăng nhập hoặc mật khẩu không đúng");

                // Kiểm tra xem người dùng có phải là Customer không
                if (user.Roleid != RoleType.Customer.ToRoleIdString())
                    return AuthResponse.Fail("Quyền truy cập bị từ chối. Chỉ dành cho Khách hàng.", HttpStatusCode.Forbidden);
                if (user.StatusId == UserStatusType.Banned.ToUserStatusIdString())
                    return AuthResponse.Fail("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ với quản trị viên để biết thêm thông tin.", HttpStatusCode.Forbidden);
                user.StatusId = UserStatusType.Active.ToUserStatusIdString();
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
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

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var userId = _tokenService.GetUserIdFromToken(refreshToken);
                if (userId == null)
                    return AuthResponse.Fail("Token không hợp lệ", HttpStatusCode.Unauthorized);
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return AuthResponse.Fail("Token không hợp lệ", HttpStatusCode.Unauthorized);
                if (user.RefreshToken != refreshToken)
                    return AuthResponse.Fail("Token không hợp lệ", HttpStatusCode.Unauthorized);
                if (_tokenService.IsTokenExpired(refreshToken))
                    return AuthResponse.Fail("Token đã hết hạn", HttpStatusCode.Unauthorized);
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                return AuthResponse.RefreshSuccess(newAccessToken, "Refresh token thành công");

            }
            catch (SecurityTokenException ex)
            {
                return AuthResponse.Fail($"Token không hợp lệ: {ex.Message}", HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                return AuthResponse.Error($"{ex.Message}");
            }
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);
            await _userRepository.UpdateRefreshTokenAsync(user.Userid, refreshToken);
            return AuthResponse.LoginSuccess(accessToken, refreshToken);
        }

        public async Task<ServiceResponse<string>> OTPAuthAsync(string email)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _userRepository.GetUserByEmailAsync(email);

                ServiceResponse<string> response;

                if (user != null)
                {
                    response = await HandleExistingUser(user, email);
                }
                else
                {
                    response = await HandleNewUser(email);
                }

                await _unitOfWork.CommitAsync();
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<string>.ErrorResponse(ex.Message);
            }
        }

        private async Task<ServiceResponse<string>> HandleExistingUser(User user, string email)
        {
            if (user.Authenticate == true)
            {
                return ServiceResponse<string>.FailResponse("Email đã được sử dụng");
            }

            if (user.CountAuth > 3)
            {
                return ServiceResponse<string>.FailResponse("Bạn đã gửi yêu cầu quá nhiều");
            }

            user.CountAuth++;
            user.Otp = GenerateOtp.Generate();
            user.VerifiedAt = DateTime.UtcNow.AddMinutes(5);
            await _emailService.SendOtpEmailAsync(
                recipientEmail: email,
                recipientName: email.Split('@')[0],
                otpCode: user.Otp
            );
            return ServiceResponse<string>.SuccessResponse("Mã OTP đã được gửi qua email của bạn");
        }

        private async Task<ServiceResponse<string>> HandleNewUser(string email)
        {
            var otp = GenerateOtp.Generate();
            var newUser = new User
            {
                Userid = Guid.NewGuid().ToString(),
                Email = email,
                Otp = otp,
                CountAuth = 1,
                VerifiedAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _userRepository.AddAsync(newUser);
            await _emailService.SendOtpEmailAsync(
                recipientEmail: email,
                recipientName: email.Split('@')[0],
                otpCode: otp
            );
            return ServiceResponse<string>.SuccessResponse("Mã OTP đã được gửi qua email của bạn");
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegistrationRequestDTO registrationRequest)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var user = await _userRepository.GetUserByEmailAsync(registrationRequest.Email);
                if (user.Authenticate == true)
                {
                    return ServiceResponse<string>.FailResponse("Email đã được sử dụng");
                }
                bool exists = await _userRepository.ExistsAsync(x =>
                    x.Username == registrationRequest.Username
                    && x.IsActive == true
                    && x.IsDeleted == false);

                if (exists)
                {
                    return ServiceResponse<string>.FailResponse("Tên đăng nhập đã tồn tại");
                }

                if (registrationRequest.Otp != user.Otp)
                {
                    return ServiceResponse<string>.FailResponse("Mã OTP không đúng");
                }

                if (user.VerifiedAt < DateTime.UtcNow)
                {
                    return ServiceResponse<string>.FailResponse("Mã OTP đã hết hạn");
                }
                user.Username = registrationRequest.Username;
                user.Password = BCrypt.Net.BCrypt.HashPassword(registrationRequest.Password);
                user.Roleid = RoleType.Customer.ToRoleIdString();
                user.Authenticate = true;
                user.IsActive = true;
                user.IsDeleted = false;

                // Tạo cart mới
                var newCart = new Cart
                {
                    Cartid = user.Userid,
                    UpdatedAt = DateTime.Now
                };
                await _cartRepository.AddAsync(newCart);

                var newCustomer = _mapper.Map<Customer>(registrationRequest);
                newCustomer.Customerid = user.Userid;
                newCustomer.Cart = newCart;

                await _customerRepository.AddAsync(newCustomer);
                await _unitOfWork.CommitAsync();
                return ServiceResponse<string>.SuccessResponse("Đăng ký thành công!");

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<string>.ErrorResponse(ex.Message);
            }
        }

        public async Task<ServiceResponse<string>> LogoutAsync(string token)
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
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResponse<string>.NotFoundResponse("Người dùng không tồn tại");
                }
                user.StatusId = UserStatusType.Inactive.ToUserStatusIdString();
                user.UpdatedAt = DateTime.UtcNow;
                user.RefreshToken = null;
                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<string>.SuccessResponse("Đăng xuất thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<string>.ErrorResponse($"Lỗi khi đăng xuất: {ex.Message}");
            }
        }
    }
}
