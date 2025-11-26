using System.Text;
using Application.Models.Response;
using Application.Persistence.Repositories;
using Application.Persistence.Services;
using Microsoft.AspNetCore.Http;
using Shared.Services;

namespace Infrastructure.Persistence.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly IMongoRepository _mongoRepository;
        private readonly string _requestLogCollection;
        private readonly IAppSettingsService _appSettings;


        public LoggingService(IMongoRepository mongoRepository, IAppSettingsService appSettings)
        {
            _mongoRepository = mongoRepository;
            _appSettings = appSettings;

            var collection = _appSettings.AppSettingsInstance?.MongoDbSettings?.RequestLogCollection;
            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new InvalidOperationException("MongoDbSettings.RequestLogCollection is not configured.");
            }

            _requestLogCollection = collection;
        }


        public async Task<BaseLog> SaveRequestLog(HttpContext httpContext)
        {
            var body = await GetRequestBody(httpContext);

            var request = httpContext.Request;
            var formattedRequest = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}{body}";

            var headers = new StringBuilder();
            foreach (var header in request.Headers)
            {
                headers.Append(header.Key).Append(':').AppendLine(header.Value);
            }

            BaseLog requestLog = new()
            {
                RequestUrl = formattedRequest,
                RequestHeaders = headers.ToString(),
                RequestIP = request.HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            return await _mongoRepository.InsertRecordAsync(_requestLogCollection, requestLog);

        }


        public async Task SaveResponseLog(HttpResponse response, BaseLog requestLog, string responseBody)
        {
            requestLog.ResponseStatusCode = response.StatusCode;
            requestLog.ResponseBody = responseBody;

            var headers = new StringBuilder();
            foreach (var header in response.Headers)
            {
                headers.Append(header.Key).Append(':').AppendLine(header.Value);
            }

            requestLog.ResponseHeaders = headers.ToString();
            await _mongoRepository.UpsertRecordAsync(_requestLogCollection, requestLog.Id, requestLog);

        }



        private async Task<string> GetRequestBody(HttpContext httpContext)
        {
            if (!httpContext.Request.ContentLength.HasValue)
                return string.Empty;

            httpContext.Request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength.Value)];
            await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            var requestBodyString = Encoding.UTF8.GetString(buffer);
            return requestBodyString;
        }

    }
}
