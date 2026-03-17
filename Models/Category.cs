using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogCSharp.Models
{
    public class Category
    {
        public long Id { get; set; }

        public  string? Name { get; set; }
        public string Description { get; set; }=string.Empty;//分类描述
        public ICollection<Post> Posts { get; set; } = new List<Post>();//一个分类下可以有多个文�?
    }
}