using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task UpdateRefreshTokenAsync(string userId, string? refreshToken);
    }
}
