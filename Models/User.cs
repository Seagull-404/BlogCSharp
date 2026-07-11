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
        
        public required string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; }  = string.Empty;
                                                              
        public DateTime CreatedAt { get; set; }

        public int? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
       

    }
}