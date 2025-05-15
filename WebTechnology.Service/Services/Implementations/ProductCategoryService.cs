﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.ProductCategories;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductCategoryService(
            IProductCategoryRepository productCategoryRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách danh mục của một sản phẩm
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<ProductCategoryDTO>>> GetCategoriesByProductIdAsync(string productId)
        {
            try
            {
                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return ServiceResponse<IEnumerable<ProductCategoryDTO>>.FailResponse(
                        "Không tìm thấy sản phẩm",
                        HttpStatusCode.NotFound);
                }

                // Lấy danh sách danh mục
                var categories = await _productCategoryRepository.GetCategoriesByProductIdAsync(productId);
                return ServiceResponse<IEnumerable<ProductCategoryDTO>>.SuccessResponse(categories);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<ProductCategoryDTO>>.ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Thêm một danh mục cho sản phẩm
        /// </summary>
        public async Task<ServiceResponse<ProductCategoryDTO>> AddProductCategoryAsync(CreateProductCategoryDTO createDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetByIdAsync(createDto.ProductId);
                if (product == null)
                {
                    return ServiceResponse<ProductCategoryDTO>.FailResponse(
                        "Không tìm thấy sản phẩm",
                        HttpStatusCode.NotFound);
                }

                // Kiểm tra danh mục tồn tại
                var category = await _categoryRepository.GetByIdAsync(createDto.CategoryId);
                if (category == null)
                {
                    return ServiceResponse<ProductCategoryDTO>.FailResponse(
                        "Không tìm thấy danh mục",
                        HttpStatusCode.NotFound);
                }

                // Kiểm tra mối quan hệ đã tồn tại chưa
                var exists = await _productCategoryRepository.ExistsAsync(createDto.ProductId, createDto.CategoryId);
                if (exists)
                {
                    return ServiceResponse<ProductCategoryDTO>.FailResponse(
                        "Sản phẩm đã thuộc danh mục này",
                        HttpStatusCode.BadRequest);
                }

                // Tạo mối quan hệ mới
                var productCategory = new ProductCategory
                {
                    Id = Guid.NewGuid().ToString(),
                    Productid = createDto.ProductId,
                    Categoryid = createDto.CategoryId
                };

                await _productCategoryRepository.AddAsync(productCategory);
                await _unitOfWork.CommitAsync();

                // Trả về thông tin đã tạo
                var result = new ProductCategoryDTO
                {
                    Id = productCategory.Id,
                    ProductId = productCategory.Productid,
                    CategoryId = productCategory.Categoryid,
                    CategoryName = category.CategoryName
                };

                return ServiceResponse<ProductCategoryDTO>.SuccessResponse(result, "Thêm danh mục cho sản phẩm thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<ProductCategoryDTO>.ErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Xóa mối quan hệ giữa sản phẩm và danh mục
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteProductCategoryAsync(string productId, string categoryId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return ServiceResponse<bool>.FailResponse(
                        "Không tìm thấy sản phẩm",
                        HttpStatusCode.NotFound);
                }

                // Kiểm tra danh mục tồn tại
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return ServiceResponse<bool>.FailResponse(
                        "Không tìm thấy danh mục",
                        HttpStatusCode.NotFound);
                }

                // Xóa mối quan hệ
                var result = await _productCategoryRepository.DeleteProductCategoryAsync(productId, categoryId);
                if (!result)
                {
                    return ServiceResponse<bool>.FailResponse(
                        "Không tìm thấy mối quan hệ giữa sản phẩm và danh mục",
                        HttpStatusCode.NotFound);
                }

                await _unitOfWork.CommitAsync();
                return ServiceResponse<bool>.SuccessResponse(true, "Xóa danh mục khỏi sản phẩm thành công");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResponse<bool>.ErrorResponse(ex.Message);
            }
        }
    }
}
