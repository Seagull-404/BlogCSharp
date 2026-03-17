using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WebApplicationDemo.Models;
using WebApplicationDemo.DTOs;

namespace WebApplicationDemo.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post,PostListDto>()  //Post 映射到 PostListDto
            .ForMember(dest =>dest.AuthorName,opt => opt.MapFrom(src =>src.Author.Name))
                 .ForMember(dest =>dest.CategoryName,opt => opt.MapFrom(src => src.Category.Name))
                 .ForMember(dest =>dest.Tags,opt => opt.MapFrom(src =>src.Tags.Select(t => t.Name).ToList()));
                                             

            CreateMap<Post,PostDetailDto>()//Post -> PostDetail
                 .ForMember(dest =>dest.AuthorName,opt => opt.MapFrom(src =>src.Author.Name))
                 .ForMember(dest =>dest.CategoryName,opt => opt.MapFrom(src => src.Category.Name))
                 .ForMember(dest =>dest.Tags,opt => opt.MapFrom(src =>src.Tags.Select(t => t.Name).ToList()));

            CreateMap<CreatePostDto,Post>(); //CreatePostDto ->Post

            CreateMap<UpdatePostDto,Post>(); //UpdatePost -> Post

            
                 

             
                 
        }
    }

}