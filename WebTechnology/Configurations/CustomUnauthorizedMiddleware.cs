using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebTechnology.Service.Models;

namespace WebTechnology.Configurations
{
    public class CustomUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomUnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Response.ContentType = "application/json";
                var response = ServiceResponse<string>.FailResponse("Bạn không có quyền truy cập", HttpStatusCode.Unauthorized);
                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/json";
                var response = ServiceResponse<string>.FailResponse("Bạn không đủ quyền truy cập tài nguyên này", HttpStatusCode.Forbidden);
                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                await context.Response.WriteAsync(json);
            }
        }
    }
} 