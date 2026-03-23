using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogCSharp.Data;
using BlogCSharp.DTOs;
using BlogCSharp.Models;
using Microsoft.AspNetCore.Authorization;
using BlogCSharp.MiddleWare;
namespace BlogCSharp.Controllers
{
    // 这个控制器专门负责“文章”资源的 HTTP 接口。
    // 它当前承担的是 API 入口职责：
    // 1. 接收客户端请求
    // 2. 调用 DbContext 查询或保存数据
    // 3. 使用 AutoMapper 把实体转换成 DTO
    // 4. 返回合适的 HTTP 响应
    //
    // 注意：
    // 这里仍然是学习项目阶段，所以业务逻辑还没有完全拆到 Service 层。
    // 在企业项目里，随着复杂度上升，这些逻辑通常会进一步下沉。
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        // 数据库上下文，用来查询和保存文章、分类、标签、用户等数据。
        private readonly BlogDbContext _context;

        // AutoMapper 用来负责实体和 DTO 之间的转换，避免手工一个字段一个字段赋值。
        private readonly IMapper _mapper;

        public PostsController(BlogDbContext context, IMapper mapper)
        {
            // 构造函数通过依赖注入拿到需要的对象。
            // ASP.NET Core 会在运行时自动帮我们创建并传入这些依赖。
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PostListDto>>> GetPosts([FromQuery] PaginationParams pagination)
        {
            // 这是公开的文章列表接口。
            // 当前设计规则是：未登录用户只能看到已发布的文章，草稿和归档文章不返回。

            //构建查询
            var query = _context.Posts.Where(post => post.Status == PostStatus.Published)
                .OrderByDescending(post => post.CreatedAt);
            
            //计算总数
            var totalCount = await query.CountAsync(); 
            
            //应用分页
            var items  =  await  query.Skip((pagination.PageNumber -1) * pagination.PageSize)
                                      .Take(pagination.PageSize).ProjectTo<PostListDto>(_mapper.ConfigurationProvider)
                                      .ToListAsync();
          
           //返回分页结果
           return Ok(new PagedResult<PostListDto>
           {
               Items =  items,
               TotalCount = totalCount,
               PageNumber = pagination.PageNumber,
               PageSize = pagination.PageSize
               
           });
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<PostDetailDto>> GetPost(long id)
        {
            // 这是“获取单篇文章详情”的接口。
            // 路由中的 id 会自动绑定到方法参数 id。
            //
            // 这里同样只允许访问已发布文章，避免未公开内容被直接通过 ID 猜出来。
            var post = await _context.Posts
                // 因为详情 DTO 需要作者、分类、标签信息，
                // 所以这里把相关导航属性一起加载出来。
                .Include(post => post.Author)
                .Include(post => post.Category)
                .Include(post => post.Tags)
                // 查询指定 id 且状态为 Published 的文章。
                .FirstOrDefaultAsync(post => post.Id == id && post.Status == PostStatus.Published);

            // 如果没有查到，返回 404。
            // 这里的“没查到”既可能是文章不存在，也可能是文章不是已发布状态。
            if (post == null)
            {
               throw new Exceptions.NotFoundException("文章",id);
            }

            // 把实体转换成详情 DTO，再返回给客户端。
            var result = _mapper.Map<PostDetailDto>(post);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<PostListDto>>> SearchPosts([FromQuery]string? keyword,PaginationParams pagination)
        {
            var query = _context.Posts
            .Where(post => post.Status == PostStatus.Published)//只查询已发布文章
            .AsQueryable();

             keyword.Trim();

            if(string.IsNullOrEmpty(keyword) )
            {
                  throw new Exceptions.BusinessException("输入不能为空！");    
            }

            
           
            if (!string.IsNullOrEmpty(keyword))
            {
                 
                query = query.Where(post =>
                               post.Title.Contains(keyword)||
                               post.Content.Contains(keyword)||
                               post.Author.UserName.Contains(keyword)||
                               post.Tags.Any(t => t.Name.Contains(keyword)));
            }
            
            var totalCount = await query.CountAsync();//计算总数
            
            var items = await query.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize).ProjectTo<PostListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(new PagedResult<PostListDto>
            {
               Items =   items,
               TotalCount = totalCount,
               PageNumber = pagination.PageNumber,
               PageSize = pagination.PageSize
            });
            
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDetailDto>> CreatePost([FromBody] CreatePostDto dto)
        {
            // [ApiController] 已经会自动处理一部分模型验证，
            // 这里保留显式判断，方便你在学习阶段更直观看到验证流程。
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                
            }

            // 先验证分类是否存在。
            // 如果客户端传入一个不存在的 CategoryId，就不应该继续创建文章。
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                throw new Exceptions.NotFoundException("分类",dto.CategoryId);
            }

           
            var author = await _context.Users.OrderBy(user => user.Id).FirstOrDefaultAsync();
            if (author == null)
            {
                throw new Exceptions.NotFoundException("作者",404);
            }

            // 根据传入的 DTO 构造 Post 实体。
            // 注意：这里除了 DTO 字段，还补充了系统自己控制的字段：
            // - AuthorId / Author
            // - CreatedAt
            // - UpdatedAt
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                Category = category,    
                AuthorId = author.Id,
                Author = author,
                Status = dto.PostStatus,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.TagIds.Any())
            {
                // 只加载数据库里真实存在的标签。
                // 这样即使客户端传了不存在的 TagId，也不会把无效关系挂上去。
                var tags = await _context.Tags
                    .Where(tag => dto.TagIds.Contains(tag.Id))
                    .ToListAsync();

                post.Tags = tags;
            }

            // 把新文章加入 EF Core 跟踪，并保存到数据库。
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // 保存成功后，把实体转换成详情 DTO 返回。
            // 使用 CreatedAtAction 表达“资源已创建”，并附带获取详情的地址。
            var result = _mapper.Map<PostDetailDto>(post);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, result);
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostDto dto)
        {
            // 先检查请求体是否满足 DTO 验证规则。
            if (!ModelState.IsValid)
            {
               return  BadRequest(ModelState);
                
            }

            // 更新文章时，要把 Tags 一起加载进来。
            // 原因是后面要替换多对多关系，如果不先加载，Clear/Add 的行为就不稳定。
            var post = await _context.Posts
                .Include(existingPost => existingPost.Tags)
                .FirstOrDefaultAsync(existingPost => existingPost.Id == id);

            // 指定 id 的文章不存在时，返回 404。
            if (post == null)
            {
                throw new Exceptions.NotFoundException("文章",id);
            }

            

            // 用请求体中的值更新允许修改的字段。
            post.Title = dto.Title;
            post.Content = dto.Content;

            // CategoryId 在 Update DTO 中是可空的，
            // 这意味着“调用方可以不改分类”。
            if (dto.CategoryId.HasValue)
            {
                // 如果调用方想改分类，先验证目标分类是否真实存在。
                var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                if (category == null)
                {
                    throw new Exceptions.NotFoundException("分类",dto.CategoryId.Value);
                }

                post.CategoryId = dto.CategoryId.Value;
            }

            // 先清空当前标签关系。
            // 这样后面可以把客户端传来的标签集合作为“新的完整标签集合”重新建立。
            post.Tags.Clear();

            if (dto.TagIds.Any())
            {
                // 加载新的标签集合，并重新挂到文章上。
                var newTags = await _context.Tags
                    .Where(tag => dto.TagIds.Contains(tag.Id))
                    .ToListAsync();

                foreach (var tag in newTags)
                {
                    post.Tags.Add(tag);
                }
            }

            // 状态在更新 DTO 中也是可选的。
            // 只有客户端显式传了状态，才执行状态变更。
            if (dto.Status.HasValue)
            {
                post.Status = dto.Status.Value;
            }

            // 每次更新文章时都刷新更新时间。
            post.UpdatedAt = DateTime.UtcNow;

            // 保存所有修改到数据库。
            await _context.SaveChangesAsync();

            // 当前先返回一个简单成功结果。
            // 后续如果你想更贴近 REST 风格，也可以改成返回更新后的详情 DTO。
            return Ok(new { message = "Post updated successfully.", postId = id });
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(long id)
        {
            // 删除前先查文章是否存在。
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                throw new Exceptions.NotFoundException("文章",id);
            }

            // 查到之后执行删除，并保存到数据库。
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            // 删除成功时返回 204 No Content，表示请求成功但没有响应体。
            return NoContent();
        }
    }
}
