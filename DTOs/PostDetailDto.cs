using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogCSharp.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BlogCSharp.DTOs
{
    public class PostDetailDto
    {
      public long Id { get; set;} 
      public required string Title { get; set;}  
      public required string Content { get; set;}

      public DateTime CreatedAt { get; set;}

      public DateTime UpdatedAt { get; set;}

      public PostStatus Status {get; set;}

      public string? AuthorName { get; set;} 

      public  string? CategoryName {get; set;}

      public ICollection<string>? Tags {get; set;} = new List<string>();


    }
}