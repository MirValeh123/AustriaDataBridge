using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Constants;
using Shared.Services;
using System.IO;

namespace Shared
{
    public static class SharedDependencyInjection
    {
        public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
        {
            var appsettingsPath = configuration["Shared:AppsettingsFile"]
                                  ?? Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var adminAppsettingsPath = configuration["Shared:AdminAppsettingsFile"]
                                       ?? Path.Combine(AppContext.BaseDirectory, "appsettings.Admin.json");

            AppSettingsValues.AppsettingsFileDirectory = appsettingsPath;
            AppSettingsValues.AdminAppsettingsFileDirectory = adminAppsettingsPath;

            services.AddSingleton<IAppSettingsService, AppSettingsService>();

            return services;
        }
    }
}

