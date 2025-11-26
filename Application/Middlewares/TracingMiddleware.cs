using Application.Persistence.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        public TracingMiddleware(RequestDelegate next, ILogger<TracingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ILoggingService loggingService)
        {
            // 1. Request-i log et
            var requestLog = await loggingService.SaveRequestLog(context);

            // 2. Response body-ni capture etmək üçün hazırlıq
            var originalBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                await loggingService.SaveResponseLog(context.Response, requestLog, responseBodyText);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }

    }
}
