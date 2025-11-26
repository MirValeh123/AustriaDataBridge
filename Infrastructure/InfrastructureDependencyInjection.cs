using Application.Persistence.Repositories;
using Application.Persistence.Repositories.Base;
using Application.Persistence.Services;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Base;
using Infrastructure.Persistence.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoGuidSerialization();

            // MongoDB Settings Configuration
            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            
            if (mongoDbSettings == null)
            {
                throw new InvalidOperationException("MongoDbSettings configuration is missing or invalid.");
            }

            // Register MongoDB Settings as Options
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Register MongoDB Client
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoDbSettings.ConnectionString));

            // Register MongoDB Context
            services.AddScoped<MongoDbContext>();

            // Register Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register MongoRepository
            services.AddScoped<IMongoRepository, MongoRepository>();

            services.AddScoped<ILoggingService, LoggingService>();

           

            return services;
        }

        private static void ConfigureMongoGuidSerialization()
        {
            var guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
            try
            {
                BsonSerializer.RegisterSerializer(typeof(Guid), guidSerializer);
            }
            catch (BsonSerializationException)
            {
                // already registered, ignore
            }

            try
            {
                BsonSerializer.RegisterSerializer(typeof(Guid?), new NullableSerializer<Guid>(guidSerializer));
            }
            catch (BsonSerializationException)
            {
            }
        }
    }
}

