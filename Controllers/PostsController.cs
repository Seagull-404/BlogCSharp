using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BlogCSharp.DTOs;
using Microsoft.AspNetCore.Authorization;
using BlogCSharp.Extensions;
using BlogCSharp.Services;

namespace BlogCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PostListDto>>> GetPosts([FromQuery] PaginationParams pagination)
        {
            var result = await _postService.GetPosts(pagination);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<PostDetailDto>> GetPost(long id)
        {
            var result = await _postService.GetPost(id);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<PostListDto>>> SearchPosts([FromQuery] string? keyword
            , [FromQuery] long? CategoryId, [FromQuery] long? tagId, [FromQuery] PaginationParams pagination)
        {
            var result = await _postService.SearchPosts(keyword, CategoryId, tagId, pagination);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDetailDto>> CreatePost([FromBody] CreatePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            var result = await _postService.CreatePost(dto, userId);
            return CreatedAtAction(nameof(GetPost), new { id = result.Id }, result);
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            await _postService.UpdatePost(id, dto, userId);
            return Ok(new { message = "Post updated successfully.", postId = id });
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(long id)
        {
            var userId = User.GetUserIdOrThrow();
            await _postService.DeletePost(id, userId);
            return NoContent();
        }
    }
}