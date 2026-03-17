using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogCSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCSharp.Data
{
    public class BlogDbContext:DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)//构造函�?
        {
        }
        public DbSet<User> Users { get; set; }//用户�?
        public DbSet<Post> Posts { get; set; }//文章�?
        public DbSet<Comment> Comments { get; set; }//评论�?
        public DbSet<Category> Categories { get; set; }//分类�?
        public DbSet<Tag> Tags { get; set; }//标签�?
    }
}