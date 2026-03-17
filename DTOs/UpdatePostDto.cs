using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogCSharp.Models;
using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    public class UpdatePostDto
    {
       [Required(ErrorMessage ="标题不能为空")]
        [MaxLength(100,ErrorMessage ="标题不能超过100字符")]
        public required string Title { get; set;}
        public string? Content { get;set;}

        public long CategoryId{ get; set;} //分类

        public List<long> TagIds {get; set;} = new List<long>();

        public  PostStatus PostStatus{ get; set;} 
    }
}