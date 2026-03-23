using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogCSharp.DTOs
{
    public class PagedResult<T>
    {
        public required List<T> Items { get; set; }  //当前页的数据列表
        
        public int TotalCount { get; set; } //总记录数
        
        public int PageNumber { get; set; } //当前页码
        
        public int PageSize { get; set; } //每页大小
        
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); //总页数（计算属性）
        
        public bool HasPrevious => PageNumber > 1; //是否有上一页
        
        public bool HasNext => PageNumber < TotalPages; //是否有下一页
        
        
    }
}