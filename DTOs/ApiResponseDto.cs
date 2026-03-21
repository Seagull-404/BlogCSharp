namespace BlogCSharp.DTOs;

public class ApiResponseDto
{
    // HTTP 状态码 (200, 400, 404, 500 等)
    public int StatusCode { get; set; }
    
    // 提示信息
    public string Message { get; set; }
    
    // 是否成功
    public bool Success => StatusCode >= 200 && StatusCode < 300;
    
    // 详细错误信息（通常只在开发环境返回堆栈）
    public string? Details { get; set; }
}
