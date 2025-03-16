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
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Get Comments for a Blog Post
        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _context.Comments.Where(c => c.BlogPostId == postId).ToListAsync();
            return Ok(comments);
        }

        // 🔹 Add Comment (Subscriber/Blogger)
        [HttpPost("post/{postId}")]
        [Authorize(Roles = "Subscriber,Blogger")]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CommentDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = new Comment { Content = model.Content, UserId = userId, BlogPostId = postId };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok(comment);
        }

        // 🔹 Edit Comment (Only Owner)
        [HttpPut("{id}")]
        [Authorize(Roles = "Subscriber,Blogger")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null || comment.UserId != userId)
                return Unauthorized("You can only edit your own comments.");

            comment.Content = model.Content;
            await _context.SaveChangesAsync();
            return Ok(comment);
        }

        // 🔹 Delete Comment (Only Owner)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Subscriber,Blogger")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null || comment.UserId != userId)
                return Unauthorized("You can only delete your own comments.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok("Comment deleted successfully.");
        }
    }
}
