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
using System.Net.Http.Headers;
using Refit;

namespace Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoGuidSerialization();

            var mongoDbSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            
            if (mongoDbSettings == null)
            {
                throw new InvalidOperationException("MongoDbSettings configuration is missing or invalid.");
            }

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoDbSettings.ConnectionString));
            services.AddScoped<MongoDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IMongoRepository, MongoRepository>();
            services.AddScoped<ILoggingService, LoggingService>();

            ConfigureRefitClients(services, configuration);

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
            }

            try
            {
                BsonSerializer.RegisterSerializer(typeof(Guid?), new NullableSerializer<Guid>(guidSerializer));
            }
            catch (BsonSerializationException)
            {
            }
        }

        private static void ConfigureRefitClients(IServiceCollection services, IConfiguration configuration)
        {
            var taxografUrl = configuration["Taxograf:Url"];
            var taxografBaseUri = BuildTaxografBaseUri(taxografUrl);
            var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";

            services
                .AddRefitClient<IManufactureApiClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = taxografBaseUri;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .ConfigureHttpMessageHandlerBuilder(builder =>
                {
                    if (environment == "Development")
                    {
                        builder.PrimaryHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                        };
                    }
                });
        }

        private static Uri BuildTaxografBaseUri(string? configuredUrl)
        {
            if (string.IsNullOrWhiteSpace(configuredUrl))
            {
                throw new InvalidOperationException("Taxograf:Url configuration is missing.");
            }

            if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out var configuredUri))
            {
                throw new InvalidOperationException("Taxograf:Url configuration is invalid.");
            }

            var builder = new UriBuilder(configuredUri)
            {
                Query = string.Empty,
                Fragment = string.Empty
            };

            return builder.Uri;
        }
    }
}

