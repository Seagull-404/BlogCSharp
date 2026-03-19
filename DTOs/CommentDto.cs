
namespace BlogCSharp.DTOs
{
    public class CommentDto
    {
        public required long Id { get; set; }
        
        public string? Content { get; set; }
        
        //返回用户名
        public string? AuthorName { get; set; }
        
        // 【关键】子评论列表：递归使用自身类型
        // 这样前端拿到数据就是一个树状结构
        public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
        
        public DateTime CreateTime { get; set; }
    }
}