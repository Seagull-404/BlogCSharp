using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    public class LoginDto
    {
        
        public required string UserName { get; set; }
        public required string PassWord { get; set; }
    }
}
