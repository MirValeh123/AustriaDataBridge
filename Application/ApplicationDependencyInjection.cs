using Application.CCC.ExceptionHandling.Extensions;
using Application.Converters;
using Application.External.Taxograf.Models;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.ResponseHandlers;
using static Shared.Delegates.MappingDelegate;

namespace Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddExceptionHandler();
            services.AddScoped<IManufactureService, ManufactureService>();
            services.AddScoped<IControlCardRequestXmlConverter, ControlCardRequestXmlConverter>();
            services.AddScoped<TaxoqrafResponseHandler<ManufactureApiResponse, ManufactureApiResponse>>(sp =>
            {
                MapperDelagate<ManufactureApiResponse, ManufactureApiResponse> identityMapper = response => response;
                return new TaxoqrafResponseHandler<ManufactureApiResponse, ManufactureApiResponse>(identityMapper);
            });

            return services;
        }
    }
}

