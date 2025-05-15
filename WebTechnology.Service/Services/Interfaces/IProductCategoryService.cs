﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.DTOs.ProductCategories;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IProductCategoryService
    {
        /// <summary>
        /// Lấy danh sách danh mục của một sản phẩm
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <returns>Danh sách danh mục của sản phẩm</returns>
        Task<ServiceResponse<IEnumerable<ProductCategoryDTO>>> GetCategoriesByProductIdAsync(string productId);
        
        /// <summary>
        /// Thêm một danh mục cho sản phẩm
        /// </summary>
        /// <param name="createDto">Thông tin mối quan hệ sản phẩm-danh mục</param>
        /// <returns>Thông tin mối quan hệ đã tạo</returns>
        Task<ServiceResponse<ProductCategoryDTO>> AddProductCategoryAsync(CreateProductCategoryDTO createDto);
        
        /// <summary>
        /// Xóa mối quan hệ giữa sản phẩm và danh mục
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>Kết quả xóa</returns>
        Task<ServiceResponse<bool>> DeleteProductCategoryAsync(string productId, string categoryId);
    }
}
