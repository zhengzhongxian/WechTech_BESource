using System;
using System.ComponentModel.DataAnnotations;

namespace WebTechnology.Repository.DTOs.Review
{
    public class CommentDTO
    {
        public string CommentId { get; set; }
        public string ReviewId { get; set; }
        
        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung bình luận không được vượt quá 1000 ký tự")]
        public string CommentText { get; set; }
        
        public DateTime? CommentedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class CreateCommentDTO
    {
        public string ReviewId { get; set; }
        
        [Required(ErrorMessage = "Nội dung bình luận không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung bình luận không được vượt quá 1000 ký tự")]
        public string CommentText { get; set; }
    }
}
