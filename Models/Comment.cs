using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogCSharp.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public required string Content { get; set; }//评论内容
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public required Post Post { get; set; }
        public long PostId { get; set; }//评论所属文章ID
        public required User Author { get; set; }
        public long AuthorId { get; set; }//评论作者ID

        public long? ParentId { get; set; }//父评论ID
        
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();//子评论?
        public Comment? Parent { get; set; }//父评论?

    }
}