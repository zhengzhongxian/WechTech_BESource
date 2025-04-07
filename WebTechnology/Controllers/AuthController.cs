using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Users;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Only admin, customer ko được phép gửi request api này.
        /// </summary>

        [HttpPost("admin/login")]
        public async Task<ActionResult<AuthResponse>> AdminLogin([FromBody] LoginRequest request)
        {
            var response = await _authService.AdminLoginAsync(request.UserName, request.Password);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Only customer, admin ko được phép gửi request api này.
        /// </summary>

        [HttpPost("customer/login")]
        public async Task<ActionResult<AuthResponse>> CustomerLogin([FromBody] LoginRequest request)
        {
            var response = await _authService.CustomerLoginAsync(request.UserName, request.Password);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Only Customer <br/> Gửi mã OTP xác thực qua email
        /// </summary>
        [HttpPost("send-otp")]
        public async Task<ActionResult<ServiceResponse<string>>> SendOtp([FromQuery] string email)
        {
            var response = await _authService.OTPAuthAsync(email);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Only Customer <br/> Sau khi customer nhận được mã OTP qua email, customer sẽ gửi mã OTP về server để xác thực tài khoản
        /// </summary>

        [HttpPost("verify-otp")]
        public async Task<ActionResult<ServiceResponse<string>>> VerifyOtp([FromBody] RegistrationRequestDTO registrationRequest)
        {
            var response = await _authService.RegisterAsync(registrationRequest);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Sau khi accesstoken hết hạn thì gửi refresh token về server để lấy accesstoken mới (nếu refesh token còn hiệu lực), nếu refesh token hết hai thì customer sẽ phải đăng nhập lại
        /// </summary>
        /// 
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromQuery] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
