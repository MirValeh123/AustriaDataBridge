using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Application.CCC.ExceptionHandling.Handlers;
using Application.Extensions;

namespace Application.CCC.ExceptionHandling
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionHandler _exceptionHandler;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExceptionHandlerMiddleware(RequestDelegate next, ExceptionHandler exceptionHandler, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _exceptionHandler = exceptionHandler;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = Guid.NewGuid().ToString();
            var originalBodyStream = context.Response.Body;

            try
            {
                bool isApiRequest = context.Request.Path.ToString().Contains("/api/");

                if (isApiRequest)
                {
                    context.Response.ClearBody();

                    await _next(context);

                    var responseBodyText = await context.Response.CopyAndReplaceBody(originalBodyStream);
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception exception)
            {
                context.Response.ContentType = "application/json";
                await _exceptionHandler.HandleException(exception, context);

                var responseBodyText = await context.Response.CopyAndReplaceBody(originalBodyStream);
            }
        }
    }
}
