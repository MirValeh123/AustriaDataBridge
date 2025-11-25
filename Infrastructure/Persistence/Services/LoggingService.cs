//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Application.Models.Response;
//using Application.Persistence.Repositories;
//using Application.Persistence.Services;
//using Microsoft.AspNetCore.Http;

//namespace Infrastructure.Persistence.Services
//{
//    public class LoggingService : ILoggingService
//    {
//        private readonly IMongoRepository _mongoRepository;
//        private readonly IHttpContextService _httpContextService;
//        public async Task<BaseLog> SaveRequestLog(HttpContext httpContext)
//        {
//            var body = await GetRequestBody(httpContext);

//            var request = httpContext.Request;
//            var formattedRequest = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}{body}";

//            var headers = new StringBuilder();
//            foreach (var header in request.Headers)
//            {
//                headers.Append(header.Key).Append(':').AppendLine(header.Value);
//            }

//            BaseLog requestLog = new()
//            {
//                RequestUrl = formattedRequest,
//                RequestHeaders = headers.ToString(),
//                RequestIP = request.HttpContext.Connection.RemoteIpAddress?.ToString()
//            };

//            return await _mongoRepository.InsertRecordAsync(_requestLogCollection, requestLog);
//        }


//        public Task SaveResponseLog(HttpResponse response, BaseLog requestLog, string responseBody)
//        {
//            throw new NotImplementedException();
//        }



//        private async Task<string> GetRequestBody(HttpContext httpContext)
//        {
//            if (!httpContext.Request.ContentLength.HasValue)
//                return string.Empty;

//            httpContext.Request.EnableBuffering();
//            var buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength.Value)];
//            await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
//            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
//            var requestBodyString = Encoding.UTF8.GetString(buffer);
//            return requestBodyString;
//        }
//    }
//}
