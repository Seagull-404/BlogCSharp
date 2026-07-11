using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public required string OldPassword { get; set; }

        [Required]
        [MinLength(6)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public required string ConfirmPassword { get; set; }
    }
}