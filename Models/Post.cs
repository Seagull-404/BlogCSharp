using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;



namespace BlogCSharp.Models

{
    public class Post
    {
        public long Id { get; set; }
       [MaxLength(200)] 
        public string Title { get; set; } =string.Empty;


        public string Content { get; set; }=string.Empty;//文章内容
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public PostStatus Status { get; set; } //文章状�?
        
        public  User? Author  { get; set; } 
        public long AuthorId { get; set; }
        public Category? Category { get; set; }
        public long CategoryId { get; set; }
       
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();//一个文章可以有多个标签
        public  ICollection<Comment> Comments { get; set; } = new List<Comment>();//一个文章可以有多个评论
        
    }

    public enum PostStatus
    {
        Draft,
        Published,
        Archived
    }
}