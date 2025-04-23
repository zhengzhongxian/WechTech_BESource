
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.Images;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IImageRepository : IGenericRepository<Image>
    {
        Task<ImageDTO?> GetImageByOrder(string order);
    }
}
