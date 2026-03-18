using System.Text;
using BlogCSharp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;





var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// 1. 获取 JWT 配置节
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// 2. 配置认证服务
builder.Services.AddAuthentication(options =>
{
    // 设置默认的认证方案为 JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // 3. 配置 Token 验证参数
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // 验证发行者（Issuer）
        ValidateIssuer = true,
        // 验证接收者（Audience）
        ValidateAudience = true,
        // 验证 token 过期时间
        ValidateLifetime = true,
        // 验证签名密钥
        ValidateIssuerSigningKey = true,
        // 有效的发行者
        ValidIssuer = jwtSettings["Issuer"],
        // 有效的接收者
        ValidAudience = jwtSettings["Audience"],
        // 设置签名密钥
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        // 不允许任何时间偏差（token 过期立即失效）
        ClockSkew = TimeSpan.Zero

    };

    // 4. 配置 JWT Bearer 事件
    options.Events = new JwtBearerEvents
    {
        // 在接收请求时提取 Token
        OnMessageReceived = context =>
        {
            // 从请求头中获取 Authorization 头
            var token = context.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                // 将 token 传递给认证中间件
                context.Token = token;
            }

            return Task.CompletedTask;
        },
        // 认证失败时触发
        OnAuthenticationFailed = context =>
        {
            // 如果是 token 过期异常
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                // 在响应头中添加过期标记，方便前端判断
                context.Response.Headers.Append("Token-Expired", "true");
            }

            return Task.CompletedTask;
        }
    };
});



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<BlogDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                     // ?开?Swagger JSON 中间件?
    app.UseSwaggerUI(settings =>
    {
        settings.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        // 自定?UI 设置
        // settings.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        // settings.ShowExtensions();
    });
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseAuthentication();  // 认证中间件
app.UseAuthorization();    // 授权中间件

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
