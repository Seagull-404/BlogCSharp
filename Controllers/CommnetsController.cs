using System.Security.Claims;
using AutoMapper;
using BlogCSharp.Data;
using BlogCSharp.Models;
using BlogCSharp.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BlogCSharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommnetsController : ControllerBase
{
    private readonly BlogDbContext _context;
    private readonly IMapper _mapper;
    public CommnetsController(BlogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<List<CommentDto>>> GetPostComments(long postId)
    {
        var comments = await _context.Comments
            .Where(c => c.PostId == postId&& c.ParentId == null)// 只查顶级评论
            .Include(c =>c.Author)// 只查顶级评论
            .Include(c =>c.Replies) // 加载子评论
            .ThenInclude(r => r.Author)// 加载子评论的作者
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
            
        return _mapper.Map<List<CommentDto>>(comments);
    }
    
             
    [HttpPost]
    public async Task<IActionResult<CommentDto>> CreateComment(CreateCommentDto dto)
    {
        //1.获取当前用户ID
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr))
        {
            return Unauthorized();    
        }
        var userId = long.Parse(userIdStr);
        
        //2.映射基础字段
        var comment = _mapper.Map<Comment>(dto);
        
        //3.手动绑定作者
        comment.AuthorId = userId;
        
        // 4. 保存
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        // 5. 返回
        // 这里的 comment.Author 是 null，为了返回 AuthorName，我们可以手动查一次或者简单处理
        // 这里演示简单处理：直接返回映射结果，AuthorName 可能会为空
        // 最佳实践是重新从数据库查一次包含 Author 的数据
        
        var result = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync( c => c.Id == CompareMethod.Id);
           
        return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, _mapper.Map<CommentDto>(result));
    }
    
}