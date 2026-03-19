using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.Models
{
    public class Category
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MinLength(10)]
        public  string? Name { get; set; }
        [MaxLength(30)]
        public string Description { get; set; }=string.Empty;//分类描述
        public ICollection<Post> Posts { get; set; } = new List<Post>();//一个分类下可以有多个文章?
    }
}