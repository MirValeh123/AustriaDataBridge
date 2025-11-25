using Microsoft.AspNetCore.Builder;

namespace Application.CCC.ExceptionHandling.Extensions
{
    public static class ExceptionHandlerMiddlewareRegisterer
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            return app;
        }
    }
}
