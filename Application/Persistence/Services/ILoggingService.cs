using Application.Models.Response;
using Microsoft.AspNetCore.Http;

namespace Application.Persistence.Services
{
    public interface ILoggingService
    {
        Task<BaseLog> SaveRequestLog(HttpContext httpContext);
        Task SaveResponseLog(HttpResponse response, BaseLog requestLog, string responseBody);


    }
}

