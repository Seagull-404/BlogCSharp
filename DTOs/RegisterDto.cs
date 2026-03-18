using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    public class RegisterDto
    {
        [MaxLength(20)] 
        public required string UserName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(16)]
        [MinLength(8)] 
        public required string PassWord { get; set; }
        
       
    }
}
