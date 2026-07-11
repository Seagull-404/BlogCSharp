using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogCSharp.Services;
using BlogCSharp.DTOs;
using BlogCSharp.Extensions;
using BlogCSharp.Models;
using BlogCSharp.MiddleWare;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BlogCSharp.Data;

namespace BlogCSharp.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly BlogDbContext _context;

        public ProfileController(IUserInfoService userInfoService, IPostService postService, IMapper mapper, BlogDbContext context)
        {
            _userInfoService = userInfoService;
            _postService = postService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var userId = User.GetUserIdOrThrow();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
            {
                throw new Exceptions.NotFoundException("用户", userId);
            }

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPut]
        public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            var result = await _userInfoService.UpdateUserInfo(userId, dto);
            return Ok(result);
        }

        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            await _userInfoService.ChangePassword(userId, dto);
            return Ok(new { message = "密码修改成功" });
        }

        [HttpGet("posts")]
        public async Task<ActionResult<PagedResult<PostListDto>>> GetUserPosts([FromQuery] PaginationParams pagination)
        {
            var userId = User.GetUserIdOrThrow();
            var result = await _userInfoService.GetUserPosts(userId, pagination);
            return Ok(result);
        }

        [HttpPut("posts/{id:long}")]
        public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.GetUserIdOrThrow();
            await _postService.UpdatePost(id, dto, userId);
            return Ok(new { message = "文章更新成功", postId = id });
        }

        [HttpDelete("posts/{id:long}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var userId = User.GetUserIdOrThrow();
            await _postService.DeletePost(id, userId);
            return NoContent();
        }
    }
}