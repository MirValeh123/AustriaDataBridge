using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DomainDependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            // Domain layer typically doesn't have services to register
            // But if you have domain services, interfaces, or factories, register them here

            // Example:
            // services.AddScoped<IDomainService, DomainService>();
            // services.AddScoped<IDomainFactory, DomainFactory>();

            return services;
        }
    }
}

