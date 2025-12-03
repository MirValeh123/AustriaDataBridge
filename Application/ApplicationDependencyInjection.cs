using Application.CCC.ExceptionHandling.Extensions;
using Application.Converters;
using Application.External.Taxograf.Models;
using Microsoft.Extensions.DependencyInjection;
using Shared.ResponseHandlers;
using static Shared.Delegates.MappingDelegate;
using System.Text.Json;
using Application.Services.Concrete;
using Application.Services.Abstract;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddExceptionHandler();
            services.AddScoped<IManufactureService, ManufactureService>();
            services.AddScoped<IControlCardRequestXmlConverter, ControlCardRequestXmlConverter>();
            services.AddScoped<TaxoqrafResponseHandler<ManufactureApiResponse>>(sp =>
            {
                // HttpResponseMessage.Content string -> ManufactureApiResponse map-laması
                // Backend camelCase, model isə PascalCase olduğu üçün case-insensitive deserialize edirik
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                MapperDelagate<string, ManufactureApiResponse> mapper = content =>
                    JsonSerializer.Deserialize<ManufactureApiResponse>(content, jsonOptions)!;

                return new TaxoqrafResponseHandler<ManufactureApiResponse>(mapper);
            });
            services.AddSingleton<ISftpService, SftpService>();

            services.AddTransient<SftpClient>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<SftpSettings>>().Value;

                if (!string.IsNullOrEmpty(settings.PrivateKeyPath))
                {
                    var privateKeyFile = new PrivateKeyFile(settings.PrivateKeyPath);
                    var connectionInfo = new ConnectionInfo(
                        settings.Host,
                        settings.Port,
                        settings.Username,
                        new PrivateKeyAuthenticationMethod(settings.Username, privateKeyFile)
                    );
                    return new SftpClient(connectionInfo);
                }
                else
                {
                    var connectionInfo = new ConnectionInfo(
                        settings.Host,
                        settings.Port,
                        settings.Username,
                        new PasswordAuthenticationMethod(settings.Username, settings.Password)
                    );
                    return new SftpClient(connectionInfo);
                }
            });

            return services;
        }
    }
}

