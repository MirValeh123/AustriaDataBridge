using Application.CCC.ExceptionHandling.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Exception Handling
            services.AddExceptionHandler();

            // AutoMapper
            // services.AddAutoMapper(typeof(ApplicationDependencyInjection));

            // MediatR
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly));

            // FluentValidation
            // services.AddValidatorsFromAssembly(typeof(ApplicationDependencyInjection).Assembly);

            return services;
        }
    }
}

