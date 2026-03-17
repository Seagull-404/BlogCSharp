using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationDemo.DTOs
{
    public class UserDto
    {
        public long Id {get; set;}

        public required string Name {get; set;}

        public string? Email {get; set;}

        public string? Role {get; set;}

        public DateTime CreatedAt {get; set;}
    }
}