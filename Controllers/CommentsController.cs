using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogCSharp.Services;
using BlogCSharp.DTOs;
using BlogCSharp.Extensions;

namespace BlogCSharp.Controllers
{
    [ApiController]
    [Route("api/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("post/{postId:long}")]
        public async Task<ActionResult<List<CommentDto>>> GetCommentsByPost(long postId)
        {
            var comments = await _commentService.GetCommentsByPostId(postId);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            var comment = await _commentService.CreateComment(dto, userId);
            return Ok(comment);
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(long id)
        {
            var userId = User.GetUserIdOrThrow();
            await _commentService.DeleteComment(id, userId);
            return NoContent();
        }
    }
}