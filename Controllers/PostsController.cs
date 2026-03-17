using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.Models;
using AutoMapper.QueryableExtensions;

namespace BlogCSharp.Controllers
{
   [ApiController]
   [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IMapper _mapper;

        public PostsController(BlogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostListDto>>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Status == PostStatus.Published)
                .OrderByDescending(post => post.CreatedAt)
                .ProjectTo<PostListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<PostDetailDto>> GetPost(long id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id && p.Status == PostStatus.Published);

            if (post == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<PostDetailDto>(post);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PostDetailDto>> CreatedPost([FromBody] CreatePostDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category does not exist.");
            }

            return StatusCode(StatusCodes.Status501NotImplemented, "Create post is not fully implemented yet.");
        }
    }
}
