using System.ComponentModel.DataAnnotations;

namespace BlogCSharp.DTOs
{
    /// <summary>
    /// 分页请求参数类
    /// 用于接收客户端传来的分页信息（第几页、每页多少条）
    /// </summary>
    public class PaginationParams
    {
        // 最大允许的每页条数，防止客户端请求过多数据导致性能问题
        private const int MaxPageSize = 50;
        
        // 默认每页10条数据
        private int _pageSize = 10;
        
        /// <summary>
        /// 当前页码，从1开始计数
        /// 默认值为1（第一页）
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每页显示的数据条数
        /// 默认10条，最大不超过50条（通过setter限制）
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageSize
        {
            get => _pageSize;
            // 如果客户端传入的pageSize超过最大值，则使用最大值
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }           
    }
}
