using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.DTOs.ProductCategories;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ProductCategoryRepository : GenericRepository<ProductCategory>, IProductCategoryRepository
    {
        private readonly WebTech _webTech;
        public ProductCategoryRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }

        /// <summary>
        /// Lấy danh sách danh mục của một sản phẩm
        /// </summary>
        public async Task<IEnumerable<ProductCategoryDTO>> GetCategoriesByProductIdAsync(string productId)
        {
            return await _webTech.ProductCategories
                .Where(pc => pc.Productid == productId)
                .Join(_webTech.Categories,
                    pc => pc.Categoryid,
                    c => c.Categoryid,
                    (pc, c) => new ProductCategoryDTO
                    {
                        Id = pc.Id,
                        ProductId = pc.Productid,
                        CategoryId = pc.Categoryid,
                        CategoryName = c.CategoryName
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra xem một sản phẩm đã có danh mục này chưa
        /// </summary>
        public async Task<bool> ExistsAsync(string productId, string categoryId)
        {
            return await _webTech.ProductCategories
                .AnyAsync(pc => pc.Productid == productId && pc.Categoryid == categoryId);
        }

        /// <summary>
        /// Xóa mối quan hệ giữa sản phẩm và danh mục
        /// </summary>
        public async Task<bool> DeleteProductCategoryAsync(string productId, string categoryId)
        {
            var productCategory = await _webTech.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Productid == productId && pc.Categoryid == categoryId);

            if (productCategory == null)
                return false;

            _webTech.ProductCategories.Remove(productCategory);
            await _webTech.SaveChangesAsync();
            return true;
        }
    }
}
