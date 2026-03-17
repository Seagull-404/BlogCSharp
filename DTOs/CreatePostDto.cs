using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDemo.Models;
using System.ComponentModel.DataAnnotations;


namespace WebApplicationDemo.DTOs
{
    public class CreatePostDto
    {
        [Required(ErrorMessage ="标题不能为空")]
        [MaxLength(100,ErrorMessage ="标题不能超过100字符")]
       public required string Title { get; set;}
       public required string Content { get;set;}

       [Required]
       [Range(1, int.MaxValue)]  
       public long CategoryId{ get; set;} //分类

       public List<long> TagIds {get; set;} = new List<long>();

       public  PostStatus PostStatus{ get; set;} 

    }
}