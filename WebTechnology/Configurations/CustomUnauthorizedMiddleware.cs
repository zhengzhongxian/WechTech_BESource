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
            // Store the original response body stream
            var originalBody = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                // Check if the response has already started
                if (context.Response.HasStarted)
                {
                    return;
                }

                // Check status code and handle unauthorized/forbidden
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    // Reset the response
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    
                    var response = ServiceResponse<string>.FailResponse("Bạn không có quyền truy cập", HttpStatusCode.Unauthorized);
                    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    // Write the response to the original stream
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsync(json);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    // Reset the response
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ContentType = "application/json";
                    
                    var response = ServiceResponse<string>.FailResponse("Bạn không đủ quyền truy cập tài nguyên này", HttpStatusCode.Forbidden);
                    var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    // Write the response to the original stream
                    context.Response.Body = originalBody;
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    // For other status codes, write the original response
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    context.Response.Body = originalBody;
                    await memoryStream.CopyToAsync(originalBody);
                }
            }
            finally
            {
                // Ensure the original stream is restored
                context.Response.Body = originalBody;
            }
        }
    }
} 