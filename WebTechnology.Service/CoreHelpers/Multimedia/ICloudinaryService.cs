using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.CoreHelpers.Multimedia
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(string base64Image, string folder = "Product");
        Task<DeletionResult> DeleteImageAsync(string publicId);
        Task<ImageUploadResult> UpdateImageAsync(string publicId, string base64Image, string folder = "Product");
    }
}
