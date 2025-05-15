﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.ProductCategories
{
    public class ProductCategoryDTO
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CreateProductCategoryDTO
    {
        [Required(ErrorMessage = "ProductId không được để trống")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "CategoryId không được để trống")]
        public string CategoryId { get; set; }
    }
}
