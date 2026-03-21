namespace BlogCSharp.MiddleWare;

public class Exceptions
{
    public class BusinessException : Exception
    { 
        public BusinessException(string message) : base(message){}
    
    }
      
    public class NotFoundException : Exception
    {
        // 参数一：实体名字 (比如 "User")
        // 参数二：主键值 (比如 123)
        public NotFoundException(string name, object key)
            // base 是调用父类 Exception 的构造函数，把信息拼好传进去
            : base($"找不到 {name} (Id: {key})"){}
    }
    
    
}