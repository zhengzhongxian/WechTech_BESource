using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Parents
{
    public class CreateParentDTO
    {
        [Required(ErrorMessage = "Tên danh mục cha không được để trống")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên danh mục cha phải từ 2 đến 255 ký tự")]
        public string? ParentName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Độ ưu tiên phải là số không âm")]
        public int? Priority { get; set; }
    }
}
