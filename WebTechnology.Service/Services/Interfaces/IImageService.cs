using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Thêm hình ảnh mới cho sản phẩm
        /// </summary>
        /// <param name="createDto">Dữ liệu hình ảnh</param>
        /// <returns>Kết quả thêm hình ảnh</returns>
        Task<ServiceResponse<Image>> AddImageAsync(CreateImageDTO createDto);
        
        /// <summary>
        /// Cập nhật thứ tự hiển thị của hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <param name="order">Thứ tự mới</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResponse<bool>> UpdateOrderAsync(string id, string order);
        
        /// <summary>
        /// Xóa hình ảnh
        /// </summary>
        /// <param name="id">ID của hình ảnh</param>
        /// <returns>Kết quả xóa hình ảnh</returns>
        Task<ServiceResponse<bool>> DeleteImageAsync(string id);
    }
}
