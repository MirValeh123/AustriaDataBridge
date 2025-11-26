using Application.Persistence.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Middlewares
{
    public class TracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TracingMiddleware> _logger;

        public TracingMiddleware(RequestDelegate next, ILogger<TracingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context, ILoggingService loggingService)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            if (loggingService is null) throw new ArgumentNullException(nameof(loggingService));

            var requestLog = await loggingService.SaveRequestLog(context);

            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                var responseBodyText = await ReadResponseBodyAsync(responseBody);
                await loggingService.SaveResponseLog(context.Response, requestLog, responseBodyText);
                await CopyToOriginalStreamAsync(responseBody, originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static async Task<string> ReadResponseBodyAsync(Stream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(responseBody, leaveOpen: true);
            var bodyText = await reader.ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);
            return bodyText;
        }

        private static async Task CopyToOriginalStreamAsync(Stream source, Stream destination)
        {
            source.Seek(0, SeekOrigin.Begin);
            await source.CopyToAsync(destination);
        }
    }
}
