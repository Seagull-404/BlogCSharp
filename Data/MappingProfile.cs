using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlogCSharp.Models;
using BlogCSharp.DTOs;

namespace BlogCSharp.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post,PostListDto>()  //Post -> PostListDto
            .ForMember(dest =>dest.AuthorName,opt => opt.MapFrom(src =>src.Author.UserName))
                 .ForMember(dest =>dest.CategoryName,opt => opt.MapFrom(src => src.Category.Name))
                 .ForMember(dest =>dest.Tags,opt => opt.MapFrom(src =>src.Tags.Select(t => t.Name).ToList()));


            CreateMap<Post, PostDetailDto>() //Post -> PostDetail
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)
                    .ToList()));
                    
                    

            CreateMap<CreatePostDto,Post>(); //CreatePostDto ->Post
          
            CreateMap<UpdatePostDto,Post>(); //UpdatePost -> Post
            
            CreateMap<Category,CategoryDto>();//Category -> CategoryDto
            
            
            // 创建映射：DTO -> Entity
            CreateMap<CreateCategoryDto, Category>();
           
            CreateMap<Tag,TagDto>();// Tag -> TagDto
            
            CreateMap<TagDto,Tag>();
            
            CreateMap<Comment,CommentDto>()
                // 映射作者名
                .ForMember(dest => dest.AuthorName,opt => opt.MapFrom(src => src.Author.UserName))
                // 映射子评论
                // AutoMapper 会自动递归处理，因为我们也配置了 Comment -> CommentDto
                .ForMember(dest => dest.Replies,opt => opt.MapFrom(src => src.Replies));
            
            CreateMap<CreateCommentDto,Comment>();

         

             
                 
        }
    }

}