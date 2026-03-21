using System.Formats.Asn1;
using System.Security.Claims;
using BlogCSharp.Data;
using BlogCSharp.MiddleWare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace BlogCSharp.Controllers;




[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
// 关键：限制只有管理员才能访问此控制器下的所有接口
public class AdminController : ControllerBase
{
    private readonly BlogDbContext _context;
    public AdminController(BlogDbContext context)
    {
        _context = context;
    }
    
    //升级为管理员
    [HttpPost("promote/{userId}")]
    public async Task<IActionResult> PromoteToAdmin(long userId)
    {
        //1.查找用户
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exceptions.NotFoundException("用户",userId);
        }
        
        //2.检查用户是否为管理员
        if (user.Role == "Admin")
        {
                throw new Exceptions.BusinessException("该用户已经是管理员！");
        }
        
        //3.修改角色
        user.Role = "Admin";
        
        //4.保存更改
        await _context.SaveChangesAsync();
        return Ok(new {message = $"用户{user.UserName}已成功升级为管理员！"});
    }

    [HttpPost("demote/{userId}")]
    public async Task<IActionResult> DemoteToAdmin(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
                throw new Exceptions.NotFoundException("用户",userId);
        }
        // 防止管理员降级自己
        var currentAdminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (user.Id == long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        {
           throw new Exceptions.BusinessException("不能降级您自己的账号");
        }

        user.Role = "User";
        await _context.SaveChangesAsync();

        return Ok("降级成功！");
    }
}