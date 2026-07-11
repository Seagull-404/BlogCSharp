using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogCSharp.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Post? Post { get; set; }
        public long PostId { get; set; }
        public User? Author { get; set; }
        public long AuthorId { get; set; }

        public long? ParentId { get; set; }
        
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public Comment? Parent { get; set; }

    }
}