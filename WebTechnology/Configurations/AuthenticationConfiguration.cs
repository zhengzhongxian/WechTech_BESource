using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebTechnology.Repository.CoreHelpers.Enums;
using WebTechnology.Service.Models;

namespace WebTechnology.Configurations
{
    public static class AuthenticationConfiguration
    {
        public static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
                    if (jwtSettings == null)
                    {
                        throw new InvalidOperationException("JWT settings not found in configuration");
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.AccessTokenKey)),
                        ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Headers["Authorization"].ToString();
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim(ClaimTypes.Role, RoleType.Admin.ToRoleIdString()));

                options.AddPolicy("CustomerOnly", policy =>
                    policy.RequireClaim(ClaimTypes.Role, RoleType.Customer.ToRoleIdString()));

                options.AddPolicy("AdminOrCustomer", policy =>
                    policy.RequireClaim(ClaimTypes.Role, RoleType.Admin.ToRoleIdString(), RoleType.Customer.ToRoleIdString()));
                options.AddPolicy("AdminOrStaff", policy =>
                    policy.RequireClaim(ClaimTypes.Role, RoleType.Staff.ToRoleIdString(), RoleType.Admin.ToRoleIdString()));
            });
        }
    }
} 