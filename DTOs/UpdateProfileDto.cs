using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    public class UpdateProfileDto
    {
        [MaxLength(50)]
        public string? UserName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Range(0, 1)]
        public int? Gender { get; set; }

        public DateTime? Birthday { get; set; }
    }
}