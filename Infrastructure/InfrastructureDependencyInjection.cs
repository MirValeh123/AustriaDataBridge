using Application.Persistence.Repositories;
using Application.Persistence.Repositories.Base;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // MongoDB Settings Configuration
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            
            if (mongoDbSettings == null)
            {
                throw new InvalidOperationException("MongoDbSettings configuration is missing or invalid.");
            }

            // Register MongoDB Settings as Options
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Register MongoDB Client
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(mongoDbSettings.ConnectionString);
            });

            // Register MongoDB Context
            services.AddScoped<MongoDbContext>();

            // Register Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register MongoRepository
            services.AddScoped<IMongoRepository, MongoRepository>();

            // External Services (HttpClient)
            // services.AddHttpClient<IExternalService, ExternalService>();

            // Options Pattern
            // services.Configure<SomeOptions>(configuration.GetSection("SomeOptions"));

            // Serilog (if needed)
            // Log.Logger = new LoggerConfiguration()
            //     .ReadFrom.Configuration(configuration)
            //     .CreateLogger();

            return services;
        }
    }
}

