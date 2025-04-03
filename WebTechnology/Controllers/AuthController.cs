using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("admin/login")]
        public async Task<ActionResult<AuthResponse>> AdminLogin([FromBody] LoginRequest request)
        {
            var response = await _authService.AdminLoginAsync(request.UserName, request.Password);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("customer/login")]
        public async Task<ActionResult<AuthResponse>> CustomerLogin([FromBody] LoginRequest request)
        {
            var response = await _authService.CustomerLoginAsync(request.UserName, request.Password);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
