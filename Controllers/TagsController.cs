using System.Security.Claims;
using AutoMapper;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogCSharp.MiddleWare;

namespace BlogCSharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController: ControllerBase
{
    private readonly BlogDbContext _context;
    private readonly IMapper _mapper;
    
    public TagsController(BlogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
   //查看所有标签
    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetTags()
    {
        var tags = await _context.Tags.ToListAsync();
        var dtos = _mapper.Map<List<TagDto>>(tags); //映射列表：AutoMapper 会自动处理 List 映射
        return Ok(dtos);    
    }
    
    //创建标签
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddTag([FromBody] TagDto tagDto)
    {
        
        var tag = _mapper.Map<Tag>(tagDto);
        
        await _context.AddAsync(tag);
        await _context.SaveChangesAsync();
        
        return Ok("创建成功！");
            
            
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteTag(long id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new Exceptions.NotFoundException("标签", id);
        }
        
        //获取当前用户ID
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        //查找用户
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (currentUser == null)
        {
            throw new Exceptions.BusinessException("请登录！");
        }
        //判断用户是否有修改权限
        if (currentUser.Role != "Admin" )
        {
            throw new Exceptions.BusinessException("无法删除他人评论！");
        }
        
        

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        
        return Ok("删除成功！");
    }
    
    
}