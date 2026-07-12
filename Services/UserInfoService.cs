using BlogCSharp.Models;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.MiddleWare;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace BlogCSharp.Services;

public class UserInfoService : IUserInfoService
{
    private readonly BlogDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    public UserInfoService(BlogDbContext context, IMapper mapper, IRedisService redisService)
    {
        _context = context;
        _mapper = mapper;
        _redisService = redisService;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User?> GetUserById(long userId)
    {
        var cacheKey = $"user:{userId}";
        var cachedUser = await _redisService.GetAsync<User>(cacheKey);
        if (cachedUser != null)
        {
            return cachedUser;
        }

        var user = await _context.Users.FindAsync(userId);
        
        if (user != null)
        {
            await _redisService.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));
        }

        return user;
    }

    public async Task<UserDto> UpdateUserInfo(long userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exceptions.NotFoundException("用户", userId);
        }

        if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName && u.Id != userId))
            {
                throw new Exceptions.BusinessException("用户名已被占用");
            }
            user.UserName = dto.UserName;
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
            {
                throw new Exceptions.BusinessException("邮箱已被注册");
            }
            user.Email = dto.Email;
        }

        if (dto.Gender.HasValue)
        {
            user.Gender = dto.Gender.Value;
        }

        if (dto.Birthday.HasValue)
        {
            user.Birthday = dto.Birthday.Value;
        }

        await _context.SaveChangesAsync();

        await _redisService.DeleteAsync($"user:{userId}");

        return _mapper.Map<UserDto>(user);
    }

    public async Task ChangePassword(long userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exceptions.NotFoundException("用户", userId);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exceptions.BusinessException("旧密码错误");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        await _context.SaveChangesAsync();

        await _redisService.DeleteAsync($"user:{userId}");
    }

    public async Task<PagedResult<PostListDto>> GetUserPosts(long userId, PaginationParams pagination)
    {
        var cacheKey = $"user:{userId}:posts";
        var cachedResult = await _redisService.GetAsync<PagedResult<PostListDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var query = _context.Posts
            .Where(post => post.AuthorId == userId)
            .OrderByDescending(post => post.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ProjectTo<PostListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new PagedResult<PostListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };

        await _redisService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}