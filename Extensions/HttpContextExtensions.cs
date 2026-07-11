using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BlogCSharp.Extensions
{
    public static class HttpContextExtensions
    {
        public static long GetUserIdOrThrow(this HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("用户未登录或用户ID无效");
            }
            return userId;
        }

        public static long GetUserIdOrThrow(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("用户未登录或用户ID无效");
            }
            return userId;
        }
    }
}