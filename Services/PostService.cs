using BlogCSharp.Models;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.MiddleWare;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace BlogCSharp.Services;

public class PostService : IPostService
{
    private readonly BlogDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRedisService _redisService;

    private const string PostListCacheKey = "posts:list";

    public PostService(BlogDbContext context, IMapper mapper, IRedisService redisService)
    {
        _context = context;
        _mapper = mapper;
        _redisService = redisService;
    }

    public async Task<PagedResult<PostListDto>> GetPosts(PaginationParams pagination)
    {
        var cachedResult = await _redisService.GetAsync<PagedResult<PostListDto>>(PostListCacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var query = _context.Posts
            .Where(post => post.Status == PostStatus.Published)
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

        await _redisService.SetAsync(PostListCacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }

    public async Task<PostDetailDto> GetPost(long id)
    {
        var cacheKey = $"post:{id}";
        var cachedPost = await _redisService.GetAsync<PostDetailDto>(cacheKey);
        if (cachedPost != null)
        {
            return cachedPost;
        }

        var post = await _context.Posts
            .Include(post => post.Author)
            .Include(post => post.Category)
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == id && post.Status == PostStatus.Published);

        if (post == null)
        {
            throw new Exceptions.NotFoundException("文章", id);
        }

        var result = _mapper.Map<PostDetailDto>(post);
        await _redisService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }

    public async Task<PagedResult<PostListDto>> SearchPosts(string? keyword, long? categoryId, long? tagId, PaginationParams pagination)
    {
        var cacheKey = $"posts:search:{keyword}:{categoryId}:{tagId}";
        var cachedResult = await _redisService.GetAsync<PagedResult<PostListDto>>(cacheKey);
        if (cachedResult != null)
        {
            return cachedResult;
        }

        var query = _context.Posts
            .Where(post => post.Status == PostStatus.Published)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(post => post.CategoryId == categoryId.Value);
        }

        if (tagId.HasValue)
        {
            query = query.Where(post => post.Tags.Any(t => t.Id == tagId));
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(post =>
                post.Title.Contains(keyword) ||
                post.Content.Contains(keyword) ||
                post.Author.UserName.Contains(keyword) ||
                post.Tags.Any(t => t.Name.Contains(keyword)));
        }

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

    public async Task<PostDetailDto> CreatePost(CreatePostDto dto, long userId)
    {
        var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (author == null)
        {
            throw new Exceptions.NotFoundException("作者", userId);
        }

        Category? category = null;
        if (dto.CategoryId.HasValue)
        {
            category = await _context.Categories.FindAsync(dto.CategoryId.Value);
            if (category == null)
            {
                throw new Exceptions.NotFoundException("分类", dto.CategoryId.Value);
            }
        }

        var post = new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId,
            Category = category,
            AuthorId = author.Id,
            Author = author,
            Status = dto.PostStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (dto.TagIds.Any())
        {
            var tags = await _context.Tags
                .Where(tag => dto.TagIds.Contains(tag.Id))
                .ToListAsync();
            post.Tags = tags;
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        await _redisService.DeleteAsync(PostListCacheKey);

        return _mapper.Map<PostDetailDto>(post);
    }

    public async Task UpdatePost(long id, UpdatePostDto dto, long userId)
    {
        var post = await _context.Posts
            .Include(existingPost => existingPost.Tags)
            .FirstOrDefaultAsync(existingPost => existingPost.Id == id);

        if (post == null)
        {
            throw new Exceptions.NotFoundException("文章", id);
        }

        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException("请登录");
        }

        if (post.AuthorId != userId && currentUser.Role != "Admin")
        {
            throw new Exceptions.BusinessException("没有权限修改");
        }

        post.Title = dto.Title;
        post.Content = dto.Content;

        if (dto.CategoryId.HasValue)
        {
            var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
            if (category == null)
            {
                throw new Exceptions.NotFoundException("分类", dto.CategoryId.Value);
            }
            post.CategoryId = dto.CategoryId.Value;
        }

        post.Tags.Clear();

        if (dto.TagIds.Any())
        {
            var newTags = await _context.Tags
                .Where(tag => dto.TagIds.Contains(tag.Id))
                .ToListAsync();

            foreach (var tag in newTags)
            {
                post.Tags.Add(tag);
            }
        }

        if (dto.Status.HasValue)
        {
            post.Status = dto.Status.Value;
        }

        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _redisService.DeleteAsync(PostListCacheKey);
        await _redisService.DeleteAsync($"post:{id}");
    }

    public async Task DeletePost(long id, long userId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            throw new Exceptions.NotFoundException("文章", id);
        }

        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (currentUser == null)
        {
            throw new UnauthorizedAccessException("请登录");
        }

        if (currentUser.Role != "Admin" && post.AuthorId != currentUser.Id)
        {
            throw new Exceptions.BusinessException("无法删除他人文章");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        await _redisService.DeleteAsync(PostListCacheKey);
        await _redisService.DeleteAsync($"post:{id}");
    }
}