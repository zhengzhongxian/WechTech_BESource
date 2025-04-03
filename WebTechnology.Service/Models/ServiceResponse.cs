using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public IEnumerable<string>? Errors { get; set; }

        public static ServiceResponse<T> SuccessResponse(T data, string message = "")
        {
            return new ServiceResponse<T>
            {
                Data = data,
                Success = true,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static ServiceResponse<T> SuccessResponse(string message = "")
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = HttpStatusCode.OK
            };
        }

        public static ServiceResponse<T> ErrorResponse(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, IEnumerable<string>? errors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = errorMessage,
                StatusCode = statusCode,
                Errors = errors
            };
        }

        public static ServiceResponse<T> NotFoundResponse(string message = "Resource not found")
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public bool IsSuccessStatusCode => Success && ((int)StatusCode >= 200 && (int)StatusCode <= 299);
        public bool HasErrors => Errors != null && Errors.Any();
    }
}
