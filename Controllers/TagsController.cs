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

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        
        return Ok("删除成功！");
    }
    
    
}