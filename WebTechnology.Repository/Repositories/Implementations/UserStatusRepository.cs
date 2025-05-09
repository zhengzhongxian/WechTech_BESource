using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Implementations;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class UserStatusRepository : GenericRepository<UserStatus>, IUserStatusRepository
    {
        private readonly WebTech _webTech;
        public UserStatusRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }
    }
}