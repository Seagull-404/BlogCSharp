using System;
using System.Collections.Generic;
using BlogCSharp.Models;


namespace BlogCSharp.DTOs
{
    public class PostListDto
    {
        public long Id { get; set;} 
         
        public required string Title{ get; set;} 
        
        public string? AuthorName{ get; set;} 

        public string? CategoryName{ get; set;} //分类
        public ICollection<string> Tags {get;set;}=new List<string>();//标签
        
        public DateTime CreatedAt{ get; set;} 
        public DateTime UpdatedAt{ get; set;} 
        
        public PostStatus PostStatus{ get; set;}    
    }
}