using BlogCSharp.DTOs;

namespace BlogCSharp.Services
{
    public interface IPostService
    {
        Task<PagedResult<PostListDto>> GetPosts(PaginationParams pagination);
        Task<PostDetailDto> GetPost(long id);
        Task<PagedResult<PostListDto>> SearchPosts(string? keyword, long? categoryId, long? tagId, PaginationParams pagination);
        Task<PostDetailDto> CreatePost(CreatePostDto dto, long userId);
        Task UpdatePost(long id, UpdatePostDto dto, long userId);
        Task DeletePost(long id, long userId);
    }
}