using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Categories
{
    public class UpdateCategoryDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string CategoryName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Priority must be a non-negative number")]
        public int? Priority { get; set; }

        public string Parentid { get; set; }

        public string Metadata { get; set; }
    }
} 