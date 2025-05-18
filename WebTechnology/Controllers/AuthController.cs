using Microsoft.AspNetCore.Authorization;
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
        /// Dành cho admin và staff, customer không được phép gửi request api này.
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
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromQuery] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> GetUserInfo()
        {
            var token = Request.Headers["Authorization"].ToString();
            var response = await _authService.LogoutAsync(token);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra xem email đã tồn tại chưa
        /// </summary>
        /// <remarks>
        /// API này cho phép kiểm tra xem email đã tồn tại trong hệ thống chưa.
        /// Chỉ trả về true nếu tài khoản có IsDeleted != true và Authenticate == true.
        /// </remarks>
        /// <param name="email">Email cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        /// <response code="200">Trả về kết quả kiểm tra</response>
        /// <response code="400">Lỗi khi không cung cấp email</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("check-existing")]
        public async Task<IActionResult> CheckExistingUser([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { Success = false, Message = "Vui lòng cung cấp email để kiểm tra" });
            }

            var response = await _authService.CheckEmailExistsAsync(email);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra xem username đã tồn tại chưa
        /// </summary>
        /// <remarks>
        /// API này cho phép kiểm tra xem username đã tồn tại trong hệ thống chưa.
        /// Chỉ trả về true nếu tài khoản có IsDeleted != true và Authenticate == true.
        /// </remarks>
        /// <param name="username">Username cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        /// <response code="200">Trả về kết quả kiểm tra</response>
        /// <response code="400">Lỗi khi không cung cấp username</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("check-existing-username")]
        public async Task<IActionResult> CheckExistingUsername([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Success = false, Message = "Vui lòng cung cấp tên đăng nhập để kiểm tra" });
            }

            var response = await _authService.CheckUsernameExistsAsync(username);
            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Kiểm tra xem email và username đã tồn tại chưa
        /// </summary>
        /// <remarks>
        /// API này cho phép kiểm tra xem email và username đã tồn tại trong hệ thống chưa.
        /// Chỉ trả về true nếu tài khoản có IsDeleted != true và Authenticate == true.
        /// </remarks>
        /// <param name="email">Email cần kiểm tra</param>
        /// <param name="username">Username cần kiểm tra</param>
        /// <returns>Kết quả kiểm tra</returns>
        /// <response code="200">Trả về kết quả kiểm tra</response>
        /// <response code="400">Lỗi khi không cung cấp email hoặc username</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("check-existing-both")]
        public async Task<IActionResult> CheckExistingBoth([FromQuery] string email, [FromQuery] string username)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Success = false, Message = "Vui lòng cung cấp cả email và tên đăng nhập để kiểm tra" });
            }

            var response = await _authService.CheckEmailAndUsernameExistAsync(email, username);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
