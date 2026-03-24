using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs;

public class CreateCommentDto
{
    [Required] public string Content { get; set; }


    [Required] public long PostId { get; set; }

    public long? ParentId { get; set; }
    
    
}