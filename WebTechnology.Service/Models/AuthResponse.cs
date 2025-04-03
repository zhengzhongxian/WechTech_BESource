using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<string>? Errors { get; set; }

        public static AuthResponse LoginSuccess(string accessToken, string refreshToken, string message = "Đăng nhập thành công")
        {
            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Success = true,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static AuthResponse RefreshSuccess(string newAccessToken, string message = "Refresh thành công nhé FE")
        {
            return new AuthResponse
            {
                AccessToken = newAccessToken,
                Success = true,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static AuthResponse Fail(string errorMessage,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            IEnumerable<string>? errors = null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static AuthResponse NotFound(string message = "Tên đăng nhập hoặc mật khẩu không đúng")
        {
            return new AuthResponse
            {
                Success = false,
                Message = message,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public static AuthResponse Error(
            string errorMessage = "Lỗi hệ thống. Vui lòng thử lại sau.",
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            IEnumerable<string>? errors = null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public bool IsValid => Success && !string.IsNullOrEmpty(AccessToken);
        public bool HasErrors => Errors != null && Errors.Any();
    }
}
