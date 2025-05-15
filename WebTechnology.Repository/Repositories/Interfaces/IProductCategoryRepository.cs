
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.ProductCategories;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
    {
        /// <summary>
        /// Lấy danh sách danh mục của một sản phẩm
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <returns>Danh sách danh mục của sản phẩm</returns>
        Task<IEnumerable<ProductCategoryDTO>> GetCategoriesByProductIdAsync(string productId);

        /// <summary>
        /// Kiểm tra xem một sản phẩm đã có danh mục này chưa
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
        Task<bool> ExistsAsync(string productId, string categoryId);

        /// <summary>
        /// Xóa mối quan hệ giữa sản phẩm và danh mục
        /// </summary>
        /// <param name="productId">ID của sản phẩm</param>
        /// <param name="categoryId">ID của danh mục</param>
        /// <returns>True nếu xóa thành công, False nếu không tìm thấy</returns>
        Task<bool> DeleteProductCategoryAsync(string productId, string categoryId);
    }
}
