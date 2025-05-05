﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.Models
{
    public class OrderQueryRequest
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 50)]
        [Required(ErrorMessage = "PageSize không được vượt 50")]
        public int PageSize { get; set; } = 10;
        
        public string? CustomerId { get; set; }
        public string? StatusId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; } = "OrderDate";
        public bool SortAscending { get; set; } = false;
    }
}
