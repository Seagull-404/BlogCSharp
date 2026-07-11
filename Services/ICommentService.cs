namespace BlogCSharp.Services
{
    public interface ICommentService
    {
        Task<List<DTOs.CommentDto>> GetCommentsByPostId(long postId);
        Task<DTOs.CommentDto> CreateComment(DTOs.CreateCommentDto dto, long userId);
        Task DeleteComment(long id, long userId);
    }
}