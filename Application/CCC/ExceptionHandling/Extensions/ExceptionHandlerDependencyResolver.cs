using Application.CCC.ExceptionHandling.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Application.CCC.ExceptionHandling.Extensions
{
    public static class ExceptionHandlerDependencyResolver
    {
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<ExceptionHandler, HttpExceptionHandler>();
            return services;
        }
    }
}
