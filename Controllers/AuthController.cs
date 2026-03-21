using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using BlogCSharp.MiddleWare;

namespace BlogCSharp.Controllers;

[ApiController ]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly BlogDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration  _configuration;

    // 构造函数注入依赖
    public AuthController(BlogDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        //初始化密码哈希器
        _passwordHasher = new PasswordHasher<User>();
    }
    
    //用户注册
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        //1.检查用户名是否已存在
        if (await _context.Users.AnyAsync(u => u.UserName == registerDto.UserName))
        {
                throw new Exceptions.BusinessException("用户名已占用");  
        }

        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
                throw new Exceptions.BusinessException("该邮箱已被注册");
        }
        
        //2.创建新用户对象
        var user = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            Role = "User",
            CreatedAt = DateTime.Now
        };
        
        //3.对密码进行加密（核心安全步骤）
        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.PassWord);
        
        //4.保存到数据库
        _context.Users.Add(user); 
        await _context.SaveChangesAsync();
        return Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Token = GenerateJwtToken(user),
            Email = user.Email
        });
    }
    
    //用户登录
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //1.根据用户名查找
        var user =  _context.Users.SingleOrDefault(u =>u.UserName == loginDto.UserName);

        if (user == null)
        {
          throw new Exceptions.NotFoundException("用户名不存在",loginDto.UserName);
        }
        
        //2.验证密码
        //参数含义： 数据库里的Hash密码，用户输入的明文密码
        var result = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash, loginDto.PassWord);
        
            if (result ==  PasswordVerificationResult.Failed)
            {
                throw new Exceptions.BusinessException("密码错误");
                
            }
            
            //3.登录成功，放回Token
            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = GenerateJwtToken(user),
                Role = user.Role,
                Email = user.Email
            });
    }
        
        
        // 生成 JWT Token 的私有方法
        private string GenerateJwtToken(User user)
        {
            // 1. 获取配置
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            // 2. 创建密钥和签名凭证
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. 定义 Payload (声明/Claims)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 用户ID
                new Claim(ClaimTypes.Name, user.UserName),                 // 用户名
                new Claim(ClaimTypes.Role, user.Role)                      // 角色（用于权限控制）
            };

            // 4. 生成 Token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // 过期时间
                signingCredentials: creds
            );

            // 5. 返回 Token 字符串
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
            // 1. 从 Claims 中获取用户 ID
            var userId =long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (userId == null)
            {
                // 理论上如果 [Authorize] 生效，这里不会为空，但作为防御性编程保留
                throw new Exceptions.BusinessException("用户ID为空");
                
            }
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id  == userId);
            if (user == null)
            {
                // Token 有效但数据库中没用户（例如用户被删除但 Token 未过期）
                throw new Exceptions.NotFoundException("用户不存在",userId);
            }
            
            // 3. 映射到 DTO 返回（避免暴露密码等敏感字段）
            var userDto= new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);


        }
        
    }
    
    
    
