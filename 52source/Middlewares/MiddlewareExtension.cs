using Microsoft.AspNetCore.Builder;

namespace _52source.Middlewares
{
    public static class MiddlewareExtension
    {
        /// <summary>
        /// 跨域请求处理中间件
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CorsMiddleware>();
            return applicationBuilder;
        }
    }
}