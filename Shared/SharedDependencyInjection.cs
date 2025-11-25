using Microsoft.Extensions.DependencyInjection;

namespace Shared
{
    public static class SharedDependencyInjection
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            // Shared services, utilities, helpers
            // Example:
            // services.AddScoped<IJsonSerializer, JsonSerializer>();
            // services.AddScoped<ICryptoService, CryptoService>();

            return services;
        }
    }
}

