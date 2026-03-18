using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace BlogCSharp.Models
{
    public class User
    {
        public long Id { get; set ;}  
         [MaxLength(50)] 
         [Required]
        public string UserName { get; set; } = string.Empty;    

        [MaxLength(100)] 
        public string Email { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } // 存储加密后的密码，不是明文！

        public string Role { get; set; }  = string.Empty;//用户角色
                                                              
        public DateTime CreatedAt { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();//一个用户可以有多个文章
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();//一个用户可以有多个评论
       

    }
}