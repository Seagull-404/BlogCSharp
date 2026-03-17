using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationDemo.Data
{
    public class BlogDbContext:DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)//构造函数
        {
        }
        public DbSet<User> Users { get; set; }//用户表
        public DbSet<Post> Posts { get; set; }//文章表
        public DbSet<Comment> Comments { get; set; }//评论表
        public DbSet<Category> Categories { get; set; }//分类表
        public DbSet<Tag> Tags { get; set; }//标签表
    }
}