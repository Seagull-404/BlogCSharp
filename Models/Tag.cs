

using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.Models
{
    public class Tag
    {
        
        public required long Id { get; set; }
        [MaxLength(10)]
        public required string Name { get; set; } 
        public ICollection<Post> Posts { get; set; } = new List<Post>();//一个标签下可以有多个文章
    }
}