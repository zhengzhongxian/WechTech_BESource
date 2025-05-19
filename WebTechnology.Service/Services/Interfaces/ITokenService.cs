using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        string? GetUserIdFromToken(string token);
        string? GetRoleFromToken(string token);
        bool IsTokenExpired(string token);
        string? GetEmailFromToken(string token);
        string? GetNameFromToken(string token);
    }
}
