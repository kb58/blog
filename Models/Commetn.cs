using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public int BlogPostId { get; set; } = string.Empty;
    }
}
