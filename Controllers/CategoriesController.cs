using AutoMapper;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.MiddleWare;
using BlogCSharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BlogCSharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly BlogDbContext _context;
    private readonly IMapper _mapper;

    public CategoriesController(BlogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
     //查看所有分类
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        var dtos = _mapper.Map<List<CategoryDto>>(categories); // 映射列表：AutoMapper 会自动处理 List 映射
        return Ok(dtos);
    }
    //创建分类
    [HttpPost]
    [Authorize(Roles = "Admin,Author")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
         //检测分类是否已存在
        if (await _context.Categories.AnyAsync(c => c.Name == categoryDto.Name))
        {
                throw new Exceptions.BusinessException("该分类已存在！");
        }
        
        // 2. 映射：DTO -> 实体
        // 此时 category 对象没有 Id，或者 Id 为 0
        var category = _mapper.Map<Category>(categoryDto);
        
        // 3. 存入数据库
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        // 4. 映射回 DTO：实体 -> DTO
        // 注意：SaveChanges 执行后，category.Id 已经被数据库自动赋值了
        var resultDto = _mapper.Map<CategoryDto>(category);
        
        return CreatedAtAction(nameof(GetCategories), new { id = category.Id },  resultDto);
    }
    // 为了让 CreatedAtAction 工作，需要有一个根据 ID 查询的方法
   

    //删除分类
    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(long id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new Exceptions.NotFoundException("分类不存在！",id);
        }
        
        // 查到之后执行删除，并保存到数据库。
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}

     
       


    
    
    
    
    
   