using Blog.DTOs;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Authorize]
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get All Blog Posts (Accessible by Everyone)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.BlogPosts.ToListAsync();
            return Ok(posts);
        }

        // 🔹 Get Single Blog Post
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound("Post not found.");
            return Ok(post);
        }

        // 🔹 Create Blog Post (Only for Bloggers)
        [HttpPost]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> CreatePost([FromBody] BlogPostDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var blogPost = new BlogPost { Title = model.Title, Content = model.Content, AuthorId = userId };
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return Ok(blogPost);
        }

        // 🔹 Edit Blog Post (Only Author)
        [HttpPut("{id}")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] BlogPostDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _context.BlogPosts.FindAsync(id);

            if (post == null || post.AuthorId != userId)
                return Unauthorized("You can only edit your own posts.");

            post.Title = model.Title;
            post.Content = model.Content;
            await _context.SaveChangesAsync();
            return Ok(post);
        }

        // 🔹 Delete Blog Post (Only Author)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Blogger")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = await _context.BlogPosts.FindAsync(id);

            if (post == null || post.AuthorId != userId)
                return Unauthorized("You can only delete your own posts.");

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok("Post deleted successfully.");
        }
    }
}
