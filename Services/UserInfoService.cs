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

    public UserInfoService(BlogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User?> GetUserById(long userId)
    {
        return await _context.Users.FindAsync(userId);
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
    }

    public async Task<PagedResult<PostListDto>> GetUserPosts(long userId, PaginationParams pagination)
    {
        var query = _context.Posts
            .Where(post => post.AuthorId == userId)
            .OrderByDescending(post => post.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ProjectTo<PostListDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<PostListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}