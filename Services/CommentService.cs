using BlogCSharp.Models;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.MiddleWare;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BlogCSharp.Services;

public class CommentService : ICommentService
{
    private readonly BlogDbContext _context;
    private readonly IMapper _mapper;

    public CommentService(BlogDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CommentDto>> GetCommentsByPostId(long postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new Exceptions.NotFoundException("文章", postId);
        }

        var comments = await _context.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Author)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateComment(CreateCommentDto dto, long userId)
    {
        var post = await _context.Posts.FindAsync(dto.PostId);
        if (post == null)
        {
            throw new Exceptions.NotFoundException("文章", dto.PostId);
        }

        var comment = new Comment
        {
            PostId = dto.PostId,
            AuthorId = userId,
            Content = dto.Content,
            CreatedAt = DateTime.Now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await _context.Entry(comment).Reference(c => c.Author).LoadAsync();

        return _mapper.Map<CommentDto>(comment);
    }

    public async Task DeleteComment(long id, long userId)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            throw new Exceptions.NotFoundException("评论", id);
        }

        if (comment.AuthorId != userId)
        {
            throw new UnauthorizedAccessException();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}