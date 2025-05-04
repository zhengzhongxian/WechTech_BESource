using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Images;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Multimedia;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            ILogger<ImageService> logger)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<ServiceResponse<Image>> AddImageAsync(CreateImageDTO createDto)
        {
            try
            {
                // Kiểm tra sản phẩm tồn tại
                var product = await _unitOfWork.Products.GetByIdAsync(createDto.ProductId);
                if (product == null)
                {
                    return ServiceResponse<Image>.ErrorResponse(
                        "Sản phẩm không tồn tại",
                        HttpStatusCode.NotFound);
                }

                // Upload ảnh lên Cloudinary
                var uploadResult = await _cloudinaryService.UploadImageAsync(createDto.ImageData);
                if (uploadResult.Error != null)
                {
                    return ServiceResponse<Image>.ErrorResponse(
                        $"Lỗi khi upload ảnh: {uploadResult.Error.Message}",
                        HttpStatusCode.BadRequest);
                }

                // Tạo mới đối tượng Image
                var image = new Image
                {
                    Imageid = Guid.NewGuid().ToString(),
                    Productid = createDto.ProductId,
                    ImageData = uploadResult.SecureUrl.ToString(),
                    Publicid = uploadResult.PublicId,
                    Order = createDto.Order ?? "0",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Thêm vào database
                await _unitOfWork.Images.AddAsync(image);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<Image>.SuccessResponse(
                    image,
                    "Thêm ảnh thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm ảnh: {Message}", ex.Message);
                return ServiceResponse<Image>.ErrorResponse(
                    $"Lỗi khi thêm ảnh: {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteImageAsync(string id)
        {
            try
            {
                // Tìm ảnh theo id
                var image = await _unitOfWork.Images.GetByIdAsync(id);
                if (image == null)
                {
                    return ServiceResponse<bool>.ErrorResponse(
                        "Ảnh không tồn tại",
                        HttpStatusCode.NotFound);
                }

                // Xóa ảnh trên Cloudinary
                if (!string.IsNullOrEmpty(image.Publicid))
                {
                    var deleteResult = await _cloudinaryService.DeleteImageAsync(image.Publicid);
                    if (deleteResult.Error != null)
                    {
                        return ServiceResponse<bool>.ErrorResponse(
                            $"Lỗi khi xóa ảnh trên Cloudinary: {deleteResult.Error.Message}",
                            HttpStatusCode.BadRequest);
                    }
                }

                // Xóa ảnh trong database
                await _unitOfWork.Images.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(
                    true,
                    "Xóa ảnh thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ảnh: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse(
                    $"Lỗi khi xóa ảnh: {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateOrderAsync(string id, string order)
        {
            try
            {
                // Tìm ảnh theo id
                var image = await _unitOfWork.Images.GetByIdAsync(id);
                if (image == null)
                {
                    return ServiceResponse<bool>.ErrorResponse(
                        "Ảnh không tồn tại",
                        HttpStatusCode.NotFound);
                }

                // Cập nhật thứ tự hiển thị
                image.Order = order;
                image.UpdatedAt = DateTime.Now;

                // Lưu thay đổi vào database
                await _unitOfWork.Images.UpdateAsync(image);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(
                    true,
                    "Cập nhật thứ tự hiển thị thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thứ tự hiển thị: {Message}", ex.Message);
                return ServiceResponse<bool>.ErrorResponse(
                    $"Lỗi khi cập nhật thứ tự hiển thị: {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}
