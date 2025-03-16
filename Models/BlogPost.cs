using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string AuthorId { get; set; } = string.Empty;
    }
}
