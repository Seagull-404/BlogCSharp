namespace BlogCSharp.Services;
using BlogCSharp.Models;
using BlogCSharp.DTOs;

public interface IUserInfoService
{
    Task<User?> GetUserById(long userId);
    Task<UserDto> UpdateUserInfo(long userId, UpdateProfileDto dto);
    Task ChangePassword(long userId, ChangePasswordDto dto);
    Task<PagedResult<PostListDto>> GetUserPosts(long userId, PaginationParams pagination);
}